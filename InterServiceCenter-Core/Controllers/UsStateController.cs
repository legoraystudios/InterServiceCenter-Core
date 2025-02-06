using InterServiceCenter_Core.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterServiceCenter_Core.Controllers;

[ApiController]
[Route("api/us-states")]
public class UsStateController : ControllerBase
{
    private readonly InterServiceCenterContext _dbContext;
    
    public UsStateController(InterServiceCenterContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public IActionResult GetUSStates()
    {
        var response = _dbContext.IscUsStates.Select(p => new
            {
                p.Id,
                p.Code,
                p.Name
            })
            .ToList();
        
        return Ok(response);
    }
}