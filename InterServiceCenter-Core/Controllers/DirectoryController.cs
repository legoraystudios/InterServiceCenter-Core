using System.Security.AccessControl;
using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterServiceCenter_Core.Controllers;

[ApiController]
[Route("api/directory")]
public class DirectoryController : ControllerBase
{
    private readonly InterServiceCenterContext _dbContext;
    private readonly DirectoryService _directoryService;

    public DirectoryController(InterServiceCenterContext dbContext, DirectoryService directoryService)
    {
        _dbContext = dbContext;
        _directoryService = directoryService;
    }
    
    [HttpGet("department")]
    public async Task<ActionResult<PagedResponse<IscDirectorydepartment>>> GetDepartments([FromQuery] int page = 0, [FromQuery] int pageSize = 0)
    {
        
        if (page == 0 && pageSize == 0)
        {
            // Fetch all posts without pagination
            var departments = await _dbContext.IscDirectorydepartments
                .Select(p => new
                {
                    p.Id,
                    p.DepartmentName,
                    p.DepartmentDescription,
                    p.AddressNote,
                    p.FacilityPhoneNumberId,
                    p.FacilityPhoneNumber.PhoneNumber,
                    p.PhoneExtension,
                    p.FacilityId,
                    Facility = new
                    {
                        p.Facility.Id,
                        p.Facility.FacilityName,
                        p.Facility.AddressLineOne,
                        p.Facility.AddressLineTwo,
                        p.Facility.City,
                        p.Facility.State,
                        StateName = p.Facility.StateNavigation.Name,
                        StateCode = p.Facility.StateNavigation.Code,
                        p.Facility.ZipCode
                    },
                    People = p.IscDirectorypeople.Select(p => new
                    {
                        p.Id,
                        p.FirstName,
                        p.LastName,
                        p.JobPosition,
                        p.FacilityPhoneNumberId,
                        p.FacilityPhoneNumber.PhoneNumber,
                        p.PhoneExtension,
                        CorporateCellphone = p.CorporateCellphone ?? "",
                        p.Email,
                        p.DirectoryDepartmentId,
                        p.DirectoryDepartment.DepartmentName
                    })
                })
                .ToListAsync();

            return Ok(departments);
        }
        else
        {
            if (page == 0 && pageSize > 0)
            {
                return BadRequest("Invalid Page Number");
            }
            
            var totalItems = await _dbContext.IscPosts.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        
            var departments = await _dbContext.IscDirectorydepartments.Select(p => new
                {
                    p.Id,
                    p.DepartmentName,
                    p.DepartmentDescription,
                    p.AddressNote,
                    p.FacilityPhoneNumberId,
                    p.FacilityPhoneNumber.PhoneNumber,
                    p.PhoneExtension,
                    p.FacilityId,
                    Facility = new
                    {
                        p.Facility.Id,
                        p.Facility.FacilityName,
                        p.Facility.AddressLineOne,
                        p.Facility.AddressLineTwo,
                        p.Facility.City,
                        p.Facility.State,
                        StateName = p.Facility.StateNavigation.Name,
                        StateCode = p.Facility.StateNavigation.Code,
                        p.Facility.ZipCode
                    },
                    People = p.IscDirectorypeople.Select(p => new
                    {
                        p.Id,
                        p.FirstName,
                        p.LastName,
                        p.JobPosition,
                        p.FacilityPhoneNumberId,
                        p.FacilityPhoneNumber.PhoneNumber,
                        p.PhoneExtension,
                        CorporateCellphone = p.CorporateCellphone ?? "",
                        p.Email,
                        p.DirectoryDepartmentId,
                        p.DirectoryDepartment.DepartmentName
                    })
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();
        
            var response = new PagedResponse<object>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalItems,
                TotalPages = totalPages,
                Items = departments
            };
        
            return Ok(response);
        }
    }
    
    [HttpGet("department/{id}")]
    public IActionResult GetDepartmentyByID(int id)
    {
        var department = _dbContext.IscDirectorydepartments.Where(p => p.Id == id).Select(p => new
            {
                p.Id,
                p.DepartmentName,
                p.DepartmentDescription,
                p.AddressNote,
                p.FacilityPhoneNumberId,
                p.FacilityPhoneNumber.PhoneNumber,
                p.PhoneExtension,
                p.FacilityId,
                Facility = new
                {
                    p.Facility.Id,
                    p.Facility.FacilityName,
                    p.Facility.AddressLineOne,
                    p.Facility.AddressLineTwo,
                    p.Facility.City,
                    p.Facility.State,
                    StateName = p.Facility.StateNavigation.Name,
                    StateCode = p.Facility.StateNavigation.Code,
                    p.Facility.ZipCode
                },
                People = p.IscDirectorypeople.Select(p => new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    p.JobPosition,
                    p.FacilityPhoneNumberId,
                    p.FacilityPhoneNumber.PhoneNumber,
                    p.PhoneExtension,
                    CorporateCellphone = p.CorporateCellphone ?? "",
                    p.Email,
                    p.DirectoryDepartmentId,
                    p.DirectoryDepartment.DepartmentName
                })
            })
            .FirstOrDefault();
        
        return Ok(department);
    }
    
    [HttpPost("department")]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> CreateDepartment([FromBody] IscDirectorydepartment department)
    {
        var response = _directoryService.SaveDepartment(department);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    [HttpPut("department")]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> ModifyDepartment([FromBody] IscDirectorydepartment department)
    {
        var response = _directoryService.ModifyDepartment(department);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    [HttpDelete("department/{id}")]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var response = _directoryService.DeleteDepartment(id);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    [HttpGet("people")]
    public async Task<ActionResult<PagedResponse<IscDirectorydepartment>>> GetDirectoryPeople([FromQuery] int page = 0, [FromQuery] int pageSize = 0)
    {
        
        if (page == 0 && pageSize == 0)
        {
            // Fetch all posts without pagination
            var people = await _dbContext.IscDirectorypeople
                .Select(p => new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    p.JobPosition,
                    p.FacilityPhoneNumberId,
                    p.FacilityPhoneNumber.PhoneNumber,
                    p.PhoneExtension,
                    CorporateCellphone = p.CorporateCellphone ?? "",
                    p.Email,
                    p.DirectoryDepartmentId,
                    p.DirectoryDepartment
                })
                .ToListAsync();

            return Ok(people);
        }
        else
        {
            if (page == 0 && pageSize > 0)
            {
                return BadRequest("Invalid Page Number");
            }
            
            var totalItems = await _dbContext.IscPosts.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        
            var people = await _dbContext.IscDirectorypeople.Select(p => new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    p.JobPosition,
                    p.FacilityPhoneNumberId,
                    p.FacilityPhoneNumber.PhoneNumber,
                    p.PhoneExtension,
                    CorporateCellphone = p.CorporateCellphone ?? "",
                    p.Email,
                    p.DirectoryDepartmentId,
                    p.DirectoryDepartment
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();
        
            var response = new PagedResponse<object>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalItems,
                TotalPages = totalPages,
                Items = people
            };
        
            return Ok(response);
        }
    }
    
    [HttpGet("people/{id}")]
    public IActionResult GetDirectoryPeopleByID(int id)
    {
        var person = _dbContext.IscDirectorypeople.Where(p => p.Id == id).Select(p => new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                p.JobPosition,
                p.FacilityPhoneNumberId,
                p.FacilityPhoneNumber.PhoneNumber,
                p.PhoneExtension,
                CorporateCellphone = p.CorporateCellphone ?? "",
                p.Email,
                p.DirectoryDepartmentId,
                p.DirectoryDepartment
            })
            .FirstOrDefault();
        
        return Ok(person);
    }
    
    [HttpPost("people")]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> CreateDirectoryPerson([FromBody] IscDirectoryperson person)
    {
        var response = _directoryService.SaveDirectoryPerson(person);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    [HttpPut("people")]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> ModifyDirectoryPerson([FromBody] IscDirectoryperson person)
    {
        var response = _directoryService.ModifyDirectoryPerson(person);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    [HttpDelete("people/{id}")]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> DeleteDirectoryPerson(int id)
    {
        var response = _directoryService.DeletePerson(id);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    
}