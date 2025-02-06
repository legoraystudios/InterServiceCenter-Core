using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Utilities;
using InterServiceCenter_Core.Utilities.Authorization;

namespace InterServiceCenter_Core.Services;

public class DirectoryService
{
    private readonly InterServiceCenterContext _dbContext;
    private readonly JwtToken _tokenService;
    private readonly GeneralUtilities _utilities;
    
    public DirectoryService(InterServiceCenterContext dbContext, JwtToken tokenService, GeneralUtilities utilities)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _utilities = utilities;
    }

    public async Task<JsonResponse> SaveDepartment(IscDirectorydepartment department)
    {
        var checkIfDepartmentExist =
            _dbContext.IscDirectorydepartments.FirstOrDefault(p => p.DepartmentName == department.DepartmentName);

        if (checkIfDepartmentExist != null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Department Name already exist in our records." };
        }

        if (department.DepartmentName.Length == 0)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid Department Name." };
        }

        if (department.FacilityPhoneNumberId == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please select a valid Phone Number." };
        }
        
        if (department.FacilityId == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please select a valid Facility." };
        }

        var newDepartment = new IscDirectorydepartment
        {
            DepartmentName = department.DepartmentName,
            DepartmentDescription = department.DepartmentDescription,
            AddressNote = department.AddressNote,
            FacilityPhoneNumberId = department.FacilityPhoneNumberId,
            PhoneExtension = department.PhoneExtension,
            FacilityId = department.FacilityId
        };

        _dbContext.IscDirectorydepartments.Add(newDepartment);
        await _dbContext.SaveChangesAsync();
        
        return new JsonResponse { StatusCode = 200, Message = "Department created successfully!" };
    }
    
    public async Task<JsonResponse> ModifyDepartment(IscDirectorydepartment department)
    {
        var existingDepartment =
            _dbContext.IscDirectorydepartments.FirstOrDefault(p => p.Id == department.Id);
        
        var checkIfNameExist =
            _dbContext.IscDirectorydepartments.FirstOrDefault(f => f.DepartmentName == department.DepartmentName && f.Id != department.Id);

        if (existingDepartment == null)
        {
            return new JsonResponse { StatusCode = 404, Message = "ERROR: Department Name doesn't exist in our records." };
        }
        
        if (checkIfNameExist != null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Department Name already exist in our records." };
        }

        if (department.DepartmentName.Length == 0)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid Department Name." };
        }

        if (department.FacilityPhoneNumberId == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please select a valid Phone Number." };
        }
        
        if (department.FacilityId == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please select a valid Facility." };
        }

        existingDepartment.DepartmentName = department.DepartmentName;
        existingDepartment.DepartmentDescription = department.DepartmentDescription;
        existingDepartment.AddressNote = department.AddressNote;
        existingDepartment.FacilityPhoneNumberId = department.FacilityPhoneNumberId;
        existingDepartment.PhoneExtension = department.PhoneExtension;
        existingDepartment.FacilityId = department.FacilityId;

        _dbContext.IscDirectorydepartments.Update(existingDepartment);
        await _dbContext.SaveChangesAsync();
        
        return new JsonResponse { StatusCode = 200, Message = "Department modified successfully!" };
    }
    
    public async Task<JsonResponse> DeleteDepartment(int id)
    {
        var departmentToDelete = _dbContext.IscDirectorydepartments.FirstOrDefault(p => p.Id == id);
        
        if (departmentToDelete == null)
        {
            return new JsonResponse { StatusCode = 404, Message = "ERROR: Department not found in our records." };
        }
        
        _dbContext.IscDirectorydepartments.Remove(departmentToDelete);
        _dbContext.SaveChanges();
        
        return new JsonResponse { StatusCode = 200, Message = "Department deleted successfully!" };
    }
    
    public async Task<JsonResponse> SaveDirectoryPerson(IscDirectoryperson person)
    {
        if (person.FirstName.Length == 0 || person.LastName.Length == 0)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid First and Last Name." };
        }

        if (person.JobPosition.Length == 0)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please select a valid Job Position." };
        }
        
        if (person.FacilityPhoneNumberId == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please select a valid Phone Number." };
        }
        
        if (person.DirectoryDepartmentId == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please select a valid Department." };
        }

        var newPerson = new IscDirectoryperson()
        {
            FirstName = person.FirstName,
            LastName = person.LastName,
            JobPosition = person.JobPosition,
            FacilityPhoneNumberId = person.FacilityPhoneNumberId,
            PhoneExtension = person.PhoneExtension,
            CorporateCellphone = person.CorporateCellphone,
            Email = person.Email,
            DirectoryDepartmentId = person.DirectoryDepartmentId
        };

        _dbContext.IscDirectorypeople.Add(newPerson);
        await _dbContext.SaveChangesAsync();
        
        return new JsonResponse { StatusCode = 200, Message = "Person created successfully!" };
    }
    
    public async Task<JsonResponse> ModifyDirectoryPerson(IscDirectoryperson person)
    {
        var existingPerson =
            _dbContext.IscDirectorypeople.FirstOrDefault(p => p.Id == person.Id);

        if (existingPerson == null)
        {
            return new JsonResponse { StatusCode = 404, Message = "ERROR: Person doesn't exist in our records." };
        }
        
        if (person.FirstName.Length == 0 || person.LastName.Length == 0)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid First and Last Name." };
        }

        if (person.JobPosition.Length == 0)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please select a valid Job Position." };
        }
        
        if (person.FacilityPhoneNumberId == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please select a valid Phone Number." };
        }
        
        if (person.DirectoryDepartmentId == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please select a valid Department." };
        }

        existingPerson.FirstName = person.FirstName;
        existingPerson.LastName = person.LastName;
        existingPerson.JobPosition = person.JobPosition;
        existingPerson.FacilityPhoneNumberId = person.FacilityPhoneNumberId;
        existingPerson.PhoneExtension = person.PhoneExtension;
        existingPerson.CorporateCellphone = person.CorporateCellphone;
        existingPerson.Email = person.Email;
        existingPerson.DirectoryDepartmentId = person.DirectoryDepartmentId;

        _dbContext.IscDirectorypeople.Update(existingPerson);
        await _dbContext.SaveChangesAsync();
        
        return new JsonResponse { StatusCode = 200, Message = "Person modified successfully!" };
    }
    
    public async Task<JsonResponse> DeletePerson(int id)
    {
        var personToDelete = _dbContext.IscDirectorypeople.FirstOrDefault(p => p.Id == id);
        
        if (personToDelete == null)
        {
            return new JsonResponse { StatusCode = 404, Message = "ERROR: Person doesn't exist in our records." };
        }
        
        _dbContext.IscDirectorypeople.Remove(personToDelete);
        _dbContext.SaveChanges();
        
        return new JsonResponse { StatusCode = 200, Message = "Department deleted successfully!" };
    }
}