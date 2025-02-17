using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.IdentityModel.Tokens;

namespace InterServiceCenter_Core.Utilities.SMTPMail;

public class SmtpTool
{
    private readonly IConfiguration _configuration;
    
    public SmtpTool(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public JsonResponse SendMessage(MimeMessage message)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var host = smtpSettings["Host"];
        var port = int.Parse(smtpSettings["Port"] ?? 587.ToString());
        var email = smtpSettings["Email"];
        var password = smtpSettings["Password"];
        var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? false.ToString());
        
        using (var client = new SmtpClient()) {
            try
            {
                client.Connect(host, port, enableSsl);
                client.Authenticate(email, password);
                client.Send(message);
                client.Disconnect(true);
                
                return new JsonResponse { StatusCode = 200, Message = "Email sent successfully" };
            }
            catch (SmtpCommandException ex)
            {
                Console.WriteLine($"An error has occured with the SMTP Connection: {ex.Message}");
                Console.WriteLine($"SMTP Status Code: {ex.StatusCode}");
                return new JsonResponse { StatusCode = 500, Message = "An error has occured with the SMTP Connection" };
            } catch (SmtpProtocolException ex) {  
                Console.WriteLine($"An error has occured with the SMTP Connection: {ex.Message}");
                return new JsonResponse { StatusCode = 500, Message = "An error has occured with the SMTP Connection" };
            } catch (AuthenticationException ex) {
                Console.WriteLine($"An error has occured with the SMTP Connection: {ex.Message}");
                return new JsonResponse { StatusCode = 500, Message = "An error has occured with the SMTP Connection" };
            } catch (Exception ex) {
                Console.WriteLine($"An error has occured with the SMTP Connection: {ex.Message}");
                return new JsonResponse { StatusCode = 500, Message = "An error has occured with the SMTP Connection" };
            }
        }
    }
    
}