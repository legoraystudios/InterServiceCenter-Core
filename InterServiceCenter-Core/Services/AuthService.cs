using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Utilities;
using InterServiceCenter_Core.Utilities.Authorization;
using InterServiceCenter_Core.Utilities.SMTPMail;
using MimeKit;

namespace InterServiceCenter_Core.Services;

public class AuthService
{
    private readonly InterServiceCenterContext _dbContext;
    private readonly JwtToken _tokenService;
    private readonly GeneralUtilities _utilities;
    private readonly IConfiguration _configuration;
    private readonly SmtpTool _smtpTool;

    public AuthService(InterServiceCenterContext dbContext, JwtToken tokenService, GeneralUtilities utilities, IConfiguration configuration, SmtpTool smtpTool)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _utilities = utilities;
        _configuration = configuration;
        _smtpTool = smtpTool;
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
    
    public JsonResponse ForgotPassword(string email)
    {
        // Fetch account by entered email
        var account = _dbContext.IscAccounts.FirstOrDefault(acct => acct.Email == email);

        if (account == null)
            return new JsonResponse { StatusCode = 404, Message = "ERROR: We're unable to find an account with this email in our records." };
        
        var token = _utilities.GenerateRandomKey(50);
        
        var newToken = new IscPasswordresettoken
        {
            Email = email,
            Token = token,
            IsActive = true,
            ExpiresIn = DateTime.Now.AddMinutes(10),
            AccountId = account.Id
        };
        
        _dbContext.IscPasswordresettokens.Add(newToken);
        _dbContext.SaveChanges();
        
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var appSettings = _configuration.GetSection("ApplicationSettings");
        var frontEndUrl = appSettings["FrontEndWebsiteUrl"];
        var senderEmail = smtpSettings["Email"];
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress ("INTER Service Center", senderEmail));
        message.To.Add(new MailboxAddress (account.FirstName + " " + account.LastName, email));
        message.Subject = "Password Reset Request - INTER Service Center";
        
        string htmlBody = $@"
<html>
    <head>
        <title>HTML Email</title>
        <style>
            @import url('https://fonts.googleapis.com/css2?family=ArsenalSC:wght@400;700&display=swap');
            body {{ font-family: 'Roboto', sans-serif; }}
            .container {{ width: 100%; max-width: 600px; margin: 0 auto; }}
            .bg-black {{ background-color: #000; }}
            .text-center {{ text-align: center; }}
            .list-inline {{ display: inline-block; }}
            .text-light {{ color: #fff; }}
            .mt-5 {{ margin-top: 3rem; }}
            .mx-2 {{ padding: 2rem 0 2rem 0; }}
            .my-3 {{ margin: 1rem 0; }}
            .my-5 {{ margin: 3rem 0; }}
            .bg-secondary {{ background-color: #6c757d; }}
            .btn-dark {{ background-color: #343a40; color: #fff; padding: 0.5rem 1rem; text-decoration: none; border-radius: 0.25rem; }}
        </style>
    </head>
    <body>
        <div class='container bg-black text-center'>
            <img class='list-inline' src='https://legoray.com/static/media/InterServiceCenter-Logo.2038edbc54ae69e79aee.png' height='80' alt='InterServiceCenter Logo' /> 
            <img class='list-inline' src='https://legoray.com/assets/images/logo2.png' height='50' alt='Logo' />
        </div> 
        <div class='container mt-5'>
            <h1 class='text-center'>Password Reset Request</h1>
            <p>Hey Raymond, <br />
            To set up a new password to your account, click 'Reset Your Password' below, or use this link:</p>
            <div class='my-3'>
                <a href='{frontEndUrl}/admin/forgot-password/reset?token={token}'>{frontEndUrl}/admin/forgot-password/reset?token={token}</a>
            </div>
            <p>The link will expire in 10 minutes. If nothing happens after clicking, copy, and paste the link in your browser.</p>
            <div class='text-center my-5'>
                <a href='{frontEndUrl}/admin/forgot-password/reset?token={token}' class='btn-dark'>Reset your Password!</a>
            </div>
            <p>Thanks, <br />
            The INTER Fajardo Campus Team.</p>
        </div>
        <div class='container mx-2 bg-secondary text-light text-center'>
            <p>Inter American University of Puerto Rico - Fajardo Campus<br />
            Parque Batey Central, Carr 195. Fajardo, Puerto Rico</p>
        </div>
    </body>
</html>";

        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = htmlBody;
        message.Body = bodyBuilder.ToMessageBody();
        
        var smtpResponse = _smtpTool.SendMessage(message);
        
        if (smtpResponse.StatusCode != 200)
            return new JsonResponse { StatusCode = 500, Message = "ERROR: An error has occured while sending the email." };
        
        return new JsonResponse { StatusCode = 200, Message = "A confirmation email with a password reset link was sent to " + email };
    }
    
    public JsonResponse CheckToken(string token)
    {
        var resetToken = _dbContext.IscPasswordresettokens.FirstOrDefault(t => t.Token == token);
        
        if (resetToken == null || resetToken.IsActive == false)
            return new JsonResponse { StatusCode = 404, Message = "ERROR: Token not found in our records." };
        
        if (resetToken.ExpiresIn < DateTime.Now)
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Token has expired or is invalid." };
        
        return new JsonResponse { StatusCode = 200, Message = "Token is valid." };
    }

    public JsonResponse ResetPassword(string token, string password, string confirmPassword)
    {
        var resetToken = _dbContext.IscPasswordresettokens.FirstOrDefault(t => t.Token == token);
        
        if (resetToken == null || resetToken.IsActive == false)
            return new JsonResponse { StatusCode = 404, Message = "ERROR: Token not found in our records." };
        
        if (resetToken.ExpiresIn < DateTime.Now)
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Token has expired or is invalid." };
        
        if (string.IsNullOrEmpty(password))
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Password can't be empty." };
        
        if (!password.Equals(confirmPassword))
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Confirm Password doesn't match with Password field." };
        
        if (password.Length < 8)
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Password must be at least 8 characters long." };
        
        if (!password.Any(char.IsUpper))
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Password must have at least an uppercase letter." };
        
        if (!password.Any(char.IsLower))
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Password must have at least an lowercase letter." };
        
        if (!password.Any(char.IsDigit))
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Password must have at least one number." };
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        
        var account = _dbContext.IscAccounts.FirstOrDefault(acct => acct.Id == resetToken.AccountId);
        account.HashedPassword = hashedPassword;
        
        _dbContext.IscAccounts.Update(account);
        _dbContext.IscPasswordresettokens.Remove(resetToken);
        _dbContext.SaveChanges();
        
        return new JsonResponse { StatusCode = 200, Message = "Password has been reset successfully!" };
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