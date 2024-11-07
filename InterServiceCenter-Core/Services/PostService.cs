using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Utilities;
using InterServiceCenter_Core.Utilities.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace InterServiceCenter_Core.Services;

public class PostService
{
    private readonly InterServiceCenterContext _dbContext;
    private readonly FileService _fileService;
    private readonly JwtToken _token;

    public PostService(InterServiceCenterContext dbContext, JwtToken jwtToken, FileService fileService)
    {
        _dbContext = dbContext;
        _token = jwtToken;
        _fileService = fileService;
    }

    public async Task<JsonResponse> SavePost(IscPost post, IFormFile? frontBanner, string loggedEmail)
    {
        var loggedUser = await _dbContext.IscAccounts.FirstOrDefaultAsync(acct => acct.Email == loggedEmail);

        if (loggedUser == null)
            return new JsonResponse
                { StatusCode = 400, Message = "ERROR: Signed In user doesn't exist in our records." };

        if (post.Title.Length == 0)
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid title." };

        if (post.Content.Length == 0)
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid content." };

        if (frontBanner != null && !string.IsNullOrEmpty(frontBanner.ContentType))
        {
            if (frontBanner.Length > 10 * 1024 * 1024)
                return new JsonResponse { StatusCode = 400, Message = "ERROR: File size should not exceed 10 MB's." };

            string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];

            var createdImageName = await _fileService.SavePostBanner(frontBanner, allowedFileExtentions);

            post.FrontBannerFile = createdImageName;
        }
        
        var newPost = new IscPost
        {
            Title = post.Title,
            Content = post.Content,
            PublishedAt = DateTime.Now,
            PublishedBy = loggedUser.Id,
            FrontBannerFile = post.FrontBannerFile
        };

        _dbContext.IscPosts.Add(newPost);
        await _dbContext.SaveChangesAsync();

        return new JsonResponse { StatusCode = 200, Message = "Post created successfully!" };
    }
    
    public async Task<JsonResponse> ModifyPost(IscPost post, IFormFile? frontBanner, string loggedEmail)
    {
        var loggedUser = await _dbContext.IscAccounts.FirstOrDefaultAsync(acct => acct.Email == loggedEmail);
        var existingPost = await _dbContext.IscPosts.FirstOrDefaultAsync(p => p.Id == post.Id);
        
        if (loggedUser == null)
            return new JsonResponse
                { StatusCode = 400, Message = "ERROR: Signed In user doesn't exist in our records." };

        if (existingPost == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Post not found in our records." };
        }

        if (post.Title.Length == 0)
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid title." };

        if (post.Content.Length == 0)
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid content." };

        if (frontBanner != null && !string.IsNullOrEmpty(frontBanner.ContentType))
        {
            if (frontBanner.Length > 10 * 1024 * 1024)
                return new JsonResponse { StatusCode = 400, Message = "ERROR: File size should not exceed 10 MB's." };

            string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];

            if (!existingPost.FrontBannerFile.IsNullOrEmpty())
            {
                var createdImageName = await _fileService.ModifyPostBanner(frontBanner, allowedFileExtentions, existingPost.FrontBannerFile);
                post.FrontBannerFile = createdImageName;
            }
            else
            {
                var createdImageName = await _fileService.SavePostBanner(frontBanner, allowedFileExtentions);
                post.FrontBannerFile = createdImageName;
            }
        }

        existingPost.Title = post.Title;
        existingPost.Content = post.Content;
        existingPost.FrontBannerFile = post.FrontBannerFile;

        _dbContext.IscPosts.Update(existingPost);
        await _dbContext.SaveChangesAsync();

        return new JsonResponse { StatusCode = 200, Message = "Post modified successfully!" };
    }

    public JsonResponse DeletePost(int postId)
    {
        var postToDelete = _dbContext.IscPosts.FirstOrDefault(p => p.Id == postId);

        if (postToDelete == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Post not found in our records." };
        }

        if (postToDelete.FrontBannerFile != null)
        {
            _fileService.DeleteFrontBanner(postToDelete.FrontBannerFile);
        }

        _dbContext.IscPosts.Remove(postToDelete);
        _dbContext.SaveChanges();
        
        return new JsonResponse { StatusCode = 200, Message = "Post deleted successfully!" };
    }
    
    public async Task<JsonResponse> GetFrontBanner(int postId)
    {
        var post = _dbContext.IscPosts.FirstOrDefault(p => p.Id == postId);

        if (post == null)
            return new JsonResponse { StatusCode = 404, Message = "ERROR: Post not found." };

        if (post.FrontBannerFile == null)
            return new JsonResponse
                { StatusCode = 404, Message = "ERROR: Front banner picture doesn't exist in our records." };

        return new JsonResponse { StatusCode = 200, Message = post.FrontBannerFile };
    }
    
    public JsonResponse RemoveFrontBanner(int id)
    {
        var checkIfPostExist = _dbContext.IscPosts.FirstOrDefault(p => p.Id == id);

        if (checkIfPostExist == null)
            return new JsonResponse { StatusCode = 404, Message = "ERROR: Post doesn't exist in our records." };

        if (checkIfPostExist.FrontBannerFile == null)
            return new JsonResponse
                { StatusCode = 404, Message = "ERROR: Front banner picture doesn't exist in our records." };

        var deletionResponse = _fileService.DeleteFrontBanner(checkIfPostExist.FrontBannerFile);

        if (deletionResponse != 200)
            return new JsonResponse
            {
                StatusCode = 500, Message = "An error has occured while deleting the photo. Please try again later."
            };

        checkIfPostExist.FrontBannerFile = null;
        _dbContext.IscPosts.Update(checkIfPostExist);
        _dbContext.SaveChanges();

        return new JsonResponse { StatusCode = 200, Message = "Banner removed successfully!" };
    }

    public JsonResponse ModifyPost(IscPost post)
    {
        return new JsonResponse { StatusCode = 200, Message = "Post modified successfully!" };
    }
}