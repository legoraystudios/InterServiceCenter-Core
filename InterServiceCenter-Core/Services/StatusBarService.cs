using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Utilities;
using InterServiceCenter_Core.Utilities.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace InterServiceCenter_Core.Services;

public class StatusBarService
{
    private readonly InterServiceCenterContext _dbContext;
    private readonly JwtToken _token;

    public StatusBarService(InterServiceCenterContext dbContext, JwtToken token)
    {
        _dbContext = dbContext;
        _token = token;
    }

    public JsonResponse SetStatusBarProperties(IscStatusbarproperty property)
    {
        var checkIfExist = _dbContext.IscStatusbarproperties.ToList();

        if (!checkIfExist.IsNullOrEmpty())
        {
            var existingProperty = _dbContext.IscStatusbarproperties.First();

            existingProperty.MessageInterval = property.MessageInterval;
            existingProperty.StatusBarColor = property.StatusBarColor;
            existingProperty.StatusBarIcon = property.StatusBarIcon;

            _dbContext.IscStatusbarproperties.Update(existingProperty);
            _dbContext.SaveChanges();
            
            return new JsonResponse { StatusCode = 200, Message = "Status Bar configuration saved successfully!" };
        }

        var newProperty = new IscStatusbarproperty()
        {
            MessageInterval = property.MessageInterval,
            StatusBarColor = property.StatusBarColor,
            StatusBarIcon = property.StatusBarIcon,
        };

        _dbContext.IscStatusbarproperties.Update(newProperty);
        _dbContext.SaveChanges();
        
        return new JsonResponse { StatusCode = 200, Message = "Status Bar configuration saved successfully!" };
    }

    public JsonResponse SaveStatusBarMessage(IscStatusbarmessage message, string loggedEmail)
    {
        var loggedUser = _dbContext.IscAccounts.FirstOrDefault(acct => acct.Email == loggedEmail);

        if (loggedUser == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Signed In user doesn't exist in our records." };
        }

        if (message.Message.IsNullOrEmpty())
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Message can't be empty." };
        }

        var newMessage = new IscStatusbarmessage()
        {
            Message = message.Message,
            Icon = message.Icon,
            CreatedBy = loggedUser.Id,
            CreatedAt = DateTime.Now,
            ExpiresIn = message.ExpiresIn
        };

        _dbContext.IscStatusbarmessages.Add(newMessage);
        _dbContext.SaveChanges();
        
        return new JsonResponse { StatusCode = 200, Message = "Status Bar message saved successfully!" };
    }
    
    public JsonResponse ModifyStatusBarMessage(IscStatusbarmessage message, string loggedEmail)
    {
        var loggedUser = _dbContext.IscAccounts.FirstOrDefault(acct => acct.Email == loggedEmail);

        if (loggedUser == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Signed In user doesn't exist in our records." };
        }

        var existingMesage = _dbContext.IscStatusbarmessages.FirstOrDefault(m => m.Id == message.Id);

        if (existingMesage == null)
        {
            return new JsonResponse { StatusCode = 404, Message = "ERROR: Status Bar message not found in our records." };
        }

        if (message.Message.IsNullOrEmpty())
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Message can't be empty." };
        }

        existingMesage.Message = message.Message;
        existingMesage.Icon = message.Icon;
        existingMesage.ModifiedBy = loggedUser.Id;
        existingMesage.ModifiedAt = DateTime.Now;
        existingMesage.ExpiresIn = message.ExpiresIn;

        _dbContext.IscStatusbarmessages.Update(existingMesage);
        _dbContext.SaveChanges();
        
        return new JsonResponse { StatusCode = 200, Message = "Status Bar message modified successfully!" };
    }

    public JsonResponse DeleteStatusBarMessage(int id)
    {
        var messageToDelete = _dbContext.IscStatusbarmessages.FirstOrDefault(m => m.Id == id);

        if (messageToDelete == null)
        {
            return new JsonResponse { StatusCode = 404, Message = "ERROR: Status Bar message not found in our records." };
        }
        
        _dbContext.IscStatusbarmessages.Remove(messageToDelete);
        _dbContext.SaveChanges();
        
        return new JsonResponse { StatusCode = 200, Message = "Status Bar message deleted successfully!" };
    }
}