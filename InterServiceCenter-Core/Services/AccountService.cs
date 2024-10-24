using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Utilities;
using InterServiceCenter_Core.Utilities.Authorization;

namespace InterServiceCenter_Core.Services;

public class AccountService
{
    private readonly InterServiceCenterContext _dbContext;
    private readonly JwtToken _token;
    private readonly FileService _fileService;
    
    public AccountService(InterServiceCenterContext dbContext, FileService fileService)
    {
        _dbContext = dbContext;
        _fileService = fileService;
    }

    public async Task<JsonResponse> SaveProfilePhoto(AddAttachmentDTO account, string loggedEmail)
    {
        var checkIfAccountExist = _dbContext.IscAccounts.FirstOrDefault(acct => acct.Id == account.Id);

        if (checkIfAccountExist == null)
        {
            return new JsonResponse{ StatusCode = 404, Message = "ERROR: Account doesn't exist in our records."};
        }
        
        if (checkIfAccountExist.ProfilePhotoFile != null)
        {
            return new JsonResponse{ StatusCode = 404, Message = "ERROR: You already have a profile photo."};
        }

        if (loggedEmail != checkIfAccountExist.Email)
        {
            return new JsonResponse{ StatusCode = 404, Message = "ERROR: You don't have permissions to perform this action."};
        }

        if (account.ImageFile == null)
        {
            return new JsonResponse{ StatusCode = 404, Message = "ERROR: You must upload an image."};
        }

        if (account.ImageFile?.Length > 10 * 1024 * 1024)
        {
            return new JsonResponse{ StatusCode = 400, Message = "ERROR: File size should not exceed 10 MB's."};
        }
        
        string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];

        string createdImageName = await _fileService.SaveProfilePhoto(account.ImageFile, allowedFileExtentions);

        checkIfAccountExist.ProfilePhotoFile = createdImageName;

        _dbContext.IscAccounts.Update(checkIfAccountExist);
        _dbContext.SaveChanges();
        
        return new JsonResponse{ StatusCode = 200, Message = "Profile photo saved successfully!"};
    }

    public async Task<JsonResponse> ModifyProfilePhoto(AddAttachmentDTO account, string loggedEmail)
    {
        var checkIfAccountExist = _dbContext.IscAccounts.FirstOrDefault(acct => acct.Id == account.Id);

        if (checkIfAccountExist == null)
        {
            return new JsonResponse{ StatusCode = 404, Message = "ERROR: Account doesn't exist in our records."};
        }
        
        if (checkIfAccountExist.ProfilePhotoFile == null)
        {
            return new JsonResponse{ StatusCode = 404, Message = "ERROR: You already have a profile photo."};
        }

        if (loggedEmail != checkIfAccountExist.Email)
        {
            return new JsonResponse{ StatusCode = 404, Message = "ERROR: You don't have permissions to perform this action."};
        }

        if (account.ImageFile == null)
        {
            return new JsonResponse{ StatusCode = 404, Message = "ERROR: You must upload an image."};
        }

        if (account.ImageFile?.Length > 10 * 1024 * 1024)
        {
            return new JsonResponse{ StatusCode = 400, Message = "ERROR: File size should not exceed 10 MB's."};
        }
        
        string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];

        string existingFileName = checkIfAccountExist.ProfilePhotoFile;
        string createdImageName = await _fileService.ModifyProfilePhoto(account.ImageFile, allowedFileExtentions, existingFileName);

        checkIfAccountExist.ProfilePhotoFile = createdImageName;

        _dbContext.IscAccounts.Update(checkIfAccountExist);
        _dbContext.SaveChanges();
        
        return new JsonResponse{ StatusCode = 200, Message = "Profile photo modified successfully!"};
    }

    public JsonResponse RemoveProfilePhoto(int id, string loggedEmail)
    {
        var checkIfAccountExist = _dbContext.IscAccounts.FirstOrDefault(acct => acct.Id == id);

        if (checkIfAccountExist == null)
        {
            return new JsonResponse{ StatusCode = 404, Message = "ERROR: Account doesn't exist in our records."};
        }
        
        if (checkIfAccountExist.ProfilePhotoFile == null)
        {
            return new JsonResponse{ StatusCode = 404, Message = "ERROR: Profile photo doesn't exist in our records."};
        }

        if (checkIfAccountExist.Email != loggedEmail)
        {
            return new JsonResponse{ StatusCode = 404, Message = "ERROR: You don't have permissions to perform this action."};
        }

        int deletionResponse = _fileService.DeleteProfilePhoto(checkIfAccountExist.ProfilePhotoFile);

        if (deletionResponse != 200)
        {
            return new JsonResponse{ StatusCode = 500, Message = "An error has occured while deleting the photo. Please try again later."};
        }

        checkIfAccountExist.ProfilePhotoFile = null;
        _dbContext.IscAccounts.Update(checkIfAccountExist);
        _dbContext.SaveChanges();
        
        return new JsonResponse{ StatusCode = 200, Message = "Profile photo removed successfully!"};
    }
    
    public JsonResponse ModifyAccount(ModifyAccountDTO account, string loggedEmail)
    {
        var existingEmployee = _dbContext.IscAccounts.FirstOrDefault(e => e.Id == account.Id);

        var checkIfEmailExist = _dbContext.IscAccounts.FirstOrDefault(e => e.Email == account.Email);
    
        if (checkIfEmailExist != null && checkIfEmailExist.Id != account.Id)
        { 
            return new JsonResponse{ StatusCode = 404, Message = "ERROR: Email address already in use in another account."};
        }

        var checkLoggedAccount = _dbContext.IscAccounts.FirstOrDefault(e => e.Email == loggedEmail);
            
        if (checkLoggedAccount?.Role == "Admin" || checkLoggedAccount?.Role == "Super Administrator" || loggedEmail == account.Email)
        {
            
            existingEmployee.FirstName = account.FirstName;
            existingEmployee.LastName = account.LastName;
            existingEmployee.Email = account.Email;
            existingEmployee.HashedPassword = existingEmployee.HashedPassword;

            if (checkLoggedAccount?.Role == "Admin" || checkLoggedAccount?.Role == "Super Administrator" && checkLoggedAccount.Email != loggedEmail)
            {
                existingEmployee.Role = account.Role;
            } else
            {
                existingEmployee.Role = existingEmployee.Role;
            }

            _dbContext.IscAccounts.Update(existingEmployee);
            _dbContext.SaveChanges();
        
            return new JsonResponse{ StatusCode = 200, Message = "Employee modified successfully!"};
        }
        
        return new JsonResponse{ StatusCode = 401, Message = "ERROR: You don't have permissions to perform this action."};
            
    }
    
    public JsonResponse DeleteAccount(int id, string loggedEmail)
    {
        var accountToDelete = _dbContext.IscAccounts.FirstOrDefault(a => a.Id == id);
        var checkLoggedAccount = _dbContext.IscAccounts.FirstOrDefault(e => e.Email == loggedEmail);

        if (accountToDelete == null)
        {
            return new JsonResponse{ StatusCode = 404, Message = "ERROR: Employee doesn't exist in our records."};
        }

        if (checkLoggedAccount?.Role != "Admin")
        {
            return new JsonResponse{ StatusCode = 401, Message = "ERROR: You don't have permissions to perform this action."};
        }
        
        if (accountToDelete.Role == "Super Administrator")
        {
            return new JsonResponse{ StatusCode = 401, Message = "ERROR: You can't delete a Super Administrator account."};
        }

        if (loggedEmail == accountToDelete.Email)
        {
            return new JsonResponse{ StatusCode = 400, Message = "ERROR: You can't delete your own account."};
        }
        
        _dbContext.IscAccounts.Remove(accountToDelete);
        _dbContext.SaveChanges();
        
        return new JsonResponse{ StatusCode = 200, Message = "Employee deleted successfully!"};
    }
}