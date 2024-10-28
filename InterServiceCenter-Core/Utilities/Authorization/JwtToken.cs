using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InterServiceCenter_Core.Contexts;
using Microsoft.IdentityModel.Tokens;

namespace InterServiceCenter_Core.Utilities.Authorization;

public class JwtToken
{
    private readonly IConfiguration _configuration;
    private readonly InterServiceCenterContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly GeneralUtilities _utilities;

    public JwtToken(IConfiguration configuration, InterServiceCenterContext dbContext,
        IHttpContextAccessor httpContextAccessor, GeneralUtilities utilities)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _utilities = utilities;
    }

    // A deviceId is a unique ID for each session, identifying a specific device that is logged in.
    // Ideal for in case you want to force sign out a specific device.
    public string GenerateToken(string email, string role, string deviceId, string ipAddress, bool remember)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        DateTime tokenExpires;
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim("role", role),
            new Claim("deviceId", deviceId),
            new Claim("ipAddress", ipAddress),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Check if the user wants to remember the session for more time
        // (Expiration time can be adjusted in appsettings.json)
        if (remember)
            tokenExpires = DateTime.Now.AddMinutes(double.Parse(jwtSettings["RememberMeExpiryMinutes"]));
        else
            tokenExpires = DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]));

        var token = new JwtSecurityToken(
            jwtSettings["Issuer"],
            jwtSettings["Audience"],
            claims,
            expires: tokenExpires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool VerifyBearer(ClaimsIdentity identity)
    {
        // Get Device ID from the token
        var deviceIdClaim = identity.FindFirst("deviceId");
        // Get IP Address assigned to the token
        var ipAddressClaim = identity.FindFirst("ipAddress");
        // Get Email Address assigned to the token
        var emailClaim = identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

        // Verify if the Device ID on the token exist on the sessions table
        var verifySession = _dbContext.IscDevicesessions.FirstOrDefault(sess => sess.DeviceId == deviceIdClaim.Value);

        // If the session is null and/or the email doesn't match with the database
        // then failed the authorization.
        if (verifySession == null || verifySession.Email != emailClaim?.Value) return false;

        // If the Client IP address doesn't match with the token IP Address
        // then failed the authorization.
        if (ipAddressClaim?.Value != _utilities.GetClientIpAddress()) return false;

        // Otherwise, authorize the access.
        return true;
    }

    public string GetLoggedEmail(ClaimsPrincipal context)
    {
        // Get Email Address assigned to the token
        var emailClaim = context.Claims
            .First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

        if (emailClaim.Length > 0) return emailClaim;

        return "";
    }

    public string GetLoggedRole(ClaimsIdentity identity)
    {
        // Get Role assigned to the token
        var emailClaim = identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        var account = _dbContext.IscAccounts.FirstOrDefault(e => e.Email == emailClaim.Value);

        if (account?.Role == null) return "";

        return account.Role;
    }
}