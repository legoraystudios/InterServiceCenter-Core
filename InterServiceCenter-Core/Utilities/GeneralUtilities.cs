using System.Security.Cryptography;

namespace InterServiceCenter_Core.Utilities;

public class GeneralUtilities
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GeneralUtilities> _logger;

    public GeneralUtilities(IHttpContextAccessor httpContextAccessor, ILogger<GeneralUtilities> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }
    
    public string GetClientIpAddress()
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        return ipAddress;
    }
    
    public string GetDeviceName()
    {
        var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
        
        if (!string.IsNullOrEmpty(userAgent))
        {
            _logger.LogInformation("User-Agent: " + userAgent);
            return userAgent;
        }

        return "Unknown device";
    }

    public string GenerateRandomKey(int size)
    {
        byte[] randomBytes = new byte[size];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        
        return BitConverter.ToString(randomBytes).Replace("-", "").ToLower(); // Hexadecimal string
    }
}