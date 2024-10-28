using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Utilities;
using InterServiceCenter_Core.Utilities.Authorization;

namespace InterServiceCenter_Core.Services;

public class AuthService
{
    private readonly InterServiceCenterContext _dbContext;
    private readonly JwtToken _tokenService;
    private readonly GeneralUtilities _utilities;

    public AuthService(InterServiceCenterContext dbContext, JwtToken tokenService, GeneralUtilities utilities)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _utilities = utilities;
    }

    public JsonResponse LoginAccount(string email, string password, bool remember)
    {
        // Fetch account by entered email
        var account = _dbContext.IscAccounts.FirstOrDefault(acct => acct.Email == email);

        if (account == null)
            return new JsonResponse
                { StatusCode = 404, Message = "ERROR: Email and/or password not found in our records." };

        // Verify hashed password on the database
        var verifyPassword = BCrypt.Net.BCrypt.Verify(password, account.HashedPassword);

        if (!verifyPassword)
            return new JsonResponse
                { StatusCode = 404, Message = "ERROR: Email and/or password not found in our records." };

        // Get Client IP Address
        var ipAddress = _utilities.GetClientIpAddress();

        // Generate Device ID
        var deviceId = _utilities.GenerateRandomKey(16);

        // Get Device Name
        var deviceName = _utilities.GetDeviceName();

        // Generate Bearer Token
        var token = _tokenService.GenerateToken(account.Email, account.Role, deviceId, ipAddress, remember);

        var newSession = new IscDevicesession
        {
            Email = email,
            DeviceName = deviceName,
            DeviceId = deviceId,
            IpAddress = ipAddress
        };

        _dbContext.IscDevicesessions.Add(newSession);
        _dbContext.SaveChanges();

        return new JsonResponse { StatusCode = 200, Message = token };
    }

    public JsonResponse SaveAccount(string firstName, string lastName, string email, string password,
        string confirmPassword)
    {
        if (_dbContext.IscAccounts.Any(emp => emp.Email == email))
            // Email already exist on database
            return new JsonResponse
                { StatusCode = 400, Message = "ERROR: Email " + email + " already exist in our records." };

        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            // Email Address is empty or invalid
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid email address." };

        // The following if statements is for
        // check if password meets the requirements:
        //
        // - Minimum length of 8 characters
        // - Contains at least one uppercase letter
        // - Contains at least one lowercase letter
        // - Contains at least one digit

        if (string.IsNullOrEmpty(password))
            // Password is empty
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Password can't be empty." };

        if (!password.Equals(confirmPassword))
            // Confirm Password doesn't match with Password
            return new JsonResponse
                { StatusCode = 400, Message = "ERROR: Confirm Password doesn't match with Password field." };

        if (password.Length < 8)
            // Password is too short
            return new JsonResponse
                { StatusCode = 400, Message = "ERROR: Password must be at least 8 characters long." };

        if (!password.Any(char.IsUpper))
            // Password does not contain uppercase letter
            return new JsonResponse
                { StatusCode = 400, Message = "ERROR: Password must have at least an uppercase letter." };

        if (!password.Any(char.IsLower))
            // Password does not contain lowercase letter
            return new JsonResponse
                { StatusCode = 400, Message = "ERROR: Password must have at least an lowercase letter." };

        if (!password.Any(char.IsDigit))
            // Password does not contain digit
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Password must have at least one number." };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var newAccount = new IscAccount
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            HashedPassword = hashedPassword,
            CreatedAt = DateTime.Now,
            Role = "Employee"
        };

        _dbContext.IscAccounts.Add(newAccount);
        _dbContext.SaveChanges();

        return new JsonResponse { StatusCode = 200, Message = "Account created successfully!" };
    }
}