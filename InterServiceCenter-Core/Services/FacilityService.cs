using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Utilities;
using InterServiceCenter_Core.Utilities.Authorization;

namespace InterServiceCenter_Core.Services;

public class FacilityService
{
    private readonly InterServiceCenterContext _dbContext;
    private readonly JwtToken _tokenService;
    private readonly GeneralUtilities _utilities;
    
    public FacilityService(InterServiceCenterContext dbContext, JwtToken tokenService, GeneralUtilities utilities)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _utilities = utilities;
    }

    public async Task<JsonResponse> SaveFacility(IscFacility facility)
    {
        var checkIfFacilityExist =
            _dbContext.IscFacilities.FirstOrDefault(f => f.FacilityName == facility.FacilityName);

        if (checkIfFacilityExist != null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: This Facility Name already exist in our records." };
        }

        if (facility.FacilityName == "")
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid Facility Name." };
        }
        
        if (facility.AddressLineOne == "")
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid Address." };
        }
        
        if (facility.State == 0)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please select a valid State/Territory." };
        }

        if (facility.ZipCode.Length > 10 || facility.ZipCode.Length == 0)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid Zip Code." };
        }
        
        var newFacility = new IscFacility
        {
            FacilityName = facility.FacilityName,
            AddressLineOne = facility.AddressLineOne,
            AddressLineTwo = facility.AddressLineTwo,
            City = facility.City,
            State = facility.State,
            ZipCode = facility.ZipCode
        };

        _dbContext.IscFacilities.Add(newFacility);
        await _dbContext.SaveChangesAsync();

        return new JsonResponse { StatusCode = 200, Message = "Facility created successfully!" };
    }
    
    public async Task<JsonResponse> ModifyFacility(IscFacility facility)
    {
        var existingFacility = _dbContext.IscFacilities.FirstOrDefault(f => f.Id == facility.Id);
        
        var checkIfNameExist =
            _dbContext.IscFacilities.FirstOrDefault(f => f.FacilityName == facility.FacilityName && f.Id != facility.Id);

        if (existingFacility == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: This Facility doesn't exist in our records." };
        }

        if (checkIfNameExist != null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: This Facility Name already exist in our records." };
        }
        
        if (facility.FacilityName == "")
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid Facility Name." };
        }
        
        if (facility.AddressLineOne == "")
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid Address." };
        }
        
        if (facility.State == 0)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please select a valid State/Territory." };
        }

        if (facility.ZipCode.Length > 10 || facility.ZipCode.Length == 0)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid Zip Code." };
        }

        existingFacility.FacilityName = facility.FacilityName;
        existingFacility.AddressLineOne = facility.AddressLineOne;
        existingFacility.AddressLineTwo = facility.AddressLineTwo;
        existingFacility.City = facility.City;
        existingFacility.State = facility.State;
        existingFacility.ZipCode = facility.ZipCode;

        _dbContext.IscFacilities.Update(existingFacility);
        await _dbContext.SaveChangesAsync();

        return new JsonResponse { StatusCode = 200, Message = "Facility modified successfully!" };
    }

    public async Task<JsonResponse> DeleteFacility(int id)
    {
        var facilityToDelete = _dbContext.IscFacilities.FirstOrDefault(p => p.Id == id);
        
        if (facilityToDelete == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Facility not found in our records." };
        }
        
        _dbContext.IscFacilities.Remove(facilityToDelete);
        _dbContext.SaveChanges();
        
        return new JsonResponse { StatusCode = 200, Message = "Facility deleted successfully!" };
    }
    
    public async Task<JsonResponse> SavePhoneNumber(IscFacilityphonenumber phone)
    {
        var checkIfNumberExist =
            _dbContext.IscFacilityphonenumbers.FirstOrDefault(f => f.PhoneNumber == phone.PhoneNumber);

        var checkIfFacilityExist =
            _dbContext.IscFacilities.FirstOrDefault(f => f.Id == phone.FacilityId);

        if (checkIfNumberExist != null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: This Phone Number is already taken." };
        }

        if (phone.PhoneNumber.Length <= 10)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid Phone Number." };
        }
        
        if (checkIfFacilityExist == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please assign a valid Facility for the Phone Number." };
        }
        
        var newPhone = new IscFacilityphonenumber()
        {
            PhoneNumber = phone.PhoneNumber,
            FacilityId = phone.FacilityId
        };

        _dbContext.IscFacilityphonenumbers.Add(newPhone);
        await _dbContext.SaveChangesAsync();

        return new JsonResponse { StatusCode = 200, Message = "Phone Number added successfully!" };
    }
    
    public async Task<JsonResponse> ModifyPhoneNumber(IscFacilityphonenumber phone)
    {
        var existingNumber =
            _dbContext.IscFacilityphonenumbers.FirstOrDefault(f => f.Id == phone.Id);
        
        var checkIfNumberExist =
            _dbContext.IscFacilityphonenumbers.FirstOrDefault(f => f.PhoneNumber == phone.PhoneNumber && f.Id != phone.Id);
        
        var checkIfFacilityExist =
            _dbContext.IscFacilities.FirstOrDefault(f => f.Id == phone.FacilityId);

        if (checkIfNumberExist != null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: This Phone Number is already taken." };
        }

        if (phone.PhoneNumber.Length <= 10)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please enter a valid Phone Number." };
        }
        
        if (checkIfFacilityExist == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Please assign a valid Facility for the Phone Number." };
        }

        existingNumber.PhoneNumber = phone.PhoneNumber;
        existingNumber.FacilityId = phone.FacilityId;
        
        _dbContext.IscFacilityphonenumbers.Update(existingNumber);
        await _dbContext.SaveChangesAsync();

        return new JsonResponse { StatusCode = 200, Message = "Phone Number modified successfully!" };
    }
    
    public async Task<JsonResponse> DeletePhoneNumber(int id)
    {
        var numberToDelete = _dbContext.IscFacilityphonenumbers.FirstOrDefault(p => p.Id == id);
        
        if (numberToDelete == null)
        {
            return new JsonResponse { StatusCode = 400, Message = "ERROR: Phone Number not found in our records." };
        }
        
        _dbContext.IscFacilityphonenumbers.Remove(numberToDelete);
        _dbContext.SaveChanges();
        
        return new JsonResponse { StatusCode = 200, Message = "Phone Number deleted successfully!" };
    }
}