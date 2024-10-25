using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Utilities;
using InterServiceCenter_Core.Utilities.Authorization;

namespace InterServiceCenter_Core.Services;

public class PostService
{
    private readonly InterServiceCenterContext _dbContext;
    private readonly JwtToken _token;

    public PostService(InterServiceCenterContext dbContext, JwtToken jwtToken)
    {
        _dbContext = dbContext;
        _token = jwtToken;
    }
    
    public JsonResponse CreatePost(IscPost post)
    {
        
        return new JsonResponse{ StatusCode = 200, Message = "Post modified successfully!"};
    }

    public JsonResponse ModifyPost(IscPost post)
    {
        
        
        return new JsonResponse{ StatusCode = 200, Message = "Post modified successfully!"};
    }
}