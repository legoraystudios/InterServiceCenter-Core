using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Utilities;
using InterServiceCenter_Core.Utilities.Authorization;

namespace InterServiceCenter_Core.Services;

public class PostService
{
    private readonly InterServiceCenterContext _dbContext;
    private readonly JwtToken _token;
    private readonly FileService _fileService;

    public PostService(InterServiceCenterContext dbContext, JwtToken jwtToken, FileService fileService)
    {
        _dbContext = dbContext;
        _token = jwtToken;
        _fileService = fileService;
    }
    
    public async Task<JsonResponse> SavePost(IscPost post, IFormFile frontBanner, string loggedEmail)
    {
        var loggedUser = _dbContext.IscAccounts.FirstOrDefault(acct => acct.Email == loggedEmail);

        if (loggedUser == null)
        {
            return new JsonResponse{ StatusCode = 400, Message = "ERROR: Signed In user doesn't exist in our records."};
        }
        
        if (post.Title.Length == 0)
        {
            return new JsonResponse{ StatusCode = 400, Message = "ERROR: Please enter a valid title."};
        }
        
        if (post.Content.Length == 0)
        {
            return new JsonResponse{ StatusCode = 400, Message = "ERROR: Please enter a valid content."};
        }
        
        if (frontBanner.Length > 0)
        {
            if (frontBanner.Length > 10 * 1024 * 1024)
            {
                return new JsonResponse{ StatusCode = 400, Message = "ERROR: File size should not exceed 10 MB's."};
            }
            
            string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];
            
            string createdImageName = await _fileService.SavePostBanner(frontBanner, allowedFileExtentions);

            post.FrontBannerFile = createdImageName;

        }

        var newPost = new IscPost
        {
            Title = post.Title,
            Content = post.Content,
            PublishedAt = DateTime.Now,
            PublishedBy = loggedUser.Id,
            PublishedByNavigation = post.PublishedByNavigation,
            FrontBannerFile = post.FrontBannerFile
        };
        
        return new JsonResponse{ StatusCode = 200, Message = "Post modified successfully!"};
    }

    public JsonResponse ModifyPost(IscPost post)
    {
        
        
        return new JsonResponse{ StatusCode = 200, Message = "Post modified successfully!"};
    }
}