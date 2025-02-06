using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterServiceCenter_Core.Controllers;

[ApiController]
[Route("api/facility")]
public class FacilityController : ControllerBase
{
    private readonly FacilityService _facilityService;
    private readonly InterServiceCenterContext _dbContext;

    public FacilityController(FacilityService facilityService, InterServiceCenterContext dbContext)
    {
        _facilityService = facilityService;
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public async Task<ActionResult<PagedResponse<IscFacility>>> GetFacilities([FromQuery] int page = 0, [FromQuery] int pageSize = 0)
    {
        
        if (page == 0 && pageSize == 0)
        {
            // Fetch all posts without pagination
            var posts = await _dbContext.IscFacilities
                .Select(p => new
                {
                    p.Id,
                    p.FacilityName,
                    p.AddressLineOne,
                    p.AddressLineTwo,
                    p.City,
                    p.State,
                    p.StateNavigation.Code,
                    p.StateNavigation.Name,
                    p.ZipCode
                })
                .ToListAsync();

            return Ok(posts);
        }
        else
        {
            if (page == 0 && pageSize > 0)
            {
                return BadRequest("Invalid Page Number");
            }
            
            var totalItems = await _dbContext.IscPosts.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        
            var posts = await _dbContext.IscFacilities.Select(p => new
                {
                    p.Id,
                    p.FacilityName,
                    p.AddressLineOne,
                    p.AddressLineTwo,
                    p.City,
                    p.State,
                    p.StateNavigation.Code,
                    p.StateNavigation.Name,
                    p.ZipCode
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();
        
            var response = new PagedResponse<object>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalItems,
                TotalPages = totalPages,
                Items = posts
            };
        
            return Ok(response);
        }
    }
    
    [HttpGet("{id}")]
    public IActionResult GetFacilityById(int id)
    {
        var posts = _dbContext.IscFacilities.Where(p => p.Id == id).Select(p => new
            {
                p.Id,
                p.FacilityName,
                p.AddressLineOne,
                p.AddressLineTwo,
                p.City,
                p.State,
                p.StateNavigation.Code,
                p.StateNavigation.Name,
                p.ZipCode
            })
            .FirstOrDefault();
        
        return Ok(posts);
    }
    
    [HttpPost("")]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> CreateFacility([FromBody] IscFacility facility)
    {
        var response = _facilityService.SaveFacility(facility);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    [HttpPut("")]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> ModifyFacility([FromBody] IscFacility facility)
    {
        var response = _facilityService.ModifyFacility(facility);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> DeleteFacility(int id)
    {
        var response = _facilityService.DeleteFacility(id);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    [HttpGet("phone")]
    public async Task<ActionResult<PagedResponse<IscFacility>>> GetPhoneNumbers([FromQuery] int page = 0, [FromQuery] int pageSize = 0)
    {
        
        if (page == 0 && pageSize == 0)
        {
            // Fetch all posts without pagination
            var phoneNumbers = await _dbContext.IscFacilityphonenumbers
                .Select(p => new
                {
                    p.Id,
                    p.PhoneNumber,
                    p.FacilityId,
                    p.Facility.FacilityName
                })
                .ToListAsync();

            return Ok(phoneNumbers);
        }
        else
        {
            if (page == 0 && pageSize > 0)
            {
                return BadRequest("Invalid Page Number");
            }
            
            var totalItems = await _dbContext.IscPosts.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        
            var phoneNumbers = await _dbContext.IscFacilityphonenumbers.Select(p => new
                {
                    p.Id,
                    p.PhoneNumber,
                    p.FacilityId,
                    p.Facility.FacilityName
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();
        
            var response = new PagedResponse<object>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalItems,
                TotalPages = totalPages,
                Items = phoneNumbers
            };
        
            return Ok(response);
        }
    }
    
    [HttpGet("phone/{id}")]
    public IActionResult GetPhoneByID(int id)
    {
        var phoneNumber = _dbContext.IscFacilityphonenumbers.Where(p => p.Id == id).Select(p => new
            {
                p.Id,
                p.PhoneNumber,
                p.FacilityId,
                p.Facility.FacilityName,
            })
            .FirstOrDefault();
        
        return Ok(phoneNumber);
    }
    
    [HttpPost("phone")]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> AddPhoneNumber([FromBody] IscFacilityphonenumber phone)
    {
        var response = _facilityService.SavePhoneNumber(phone);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    [HttpPut("phone")]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> ModifyPhoneNumber([FromBody] IscFacilityphonenumber phone)
    {
        var response = _facilityService.ModifyPhoneNumber(phone);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    [HttpDelete("phone/{id}")]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> DeletePhoneNumber(int id)
    {
        var response = _facilityService.DeletePhoneNumber(id);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
}