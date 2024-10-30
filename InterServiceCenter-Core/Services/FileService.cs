namespace InterServiceCenter_Core.Services;

public interface IFileService
{
}

public class FileService : IFileService
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public FileService(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    public async Task<string> SaveProfilePhoto(IFormFile imageFile, string[] allowedFileExtensions)
    {
        if (imageFile == null) throw new ArgumentNullException(nameof(imageFile));

        var contentPath = _environment.ContentRootPath;
        var path = Path.Combine(contentPath, _configuration["StoragePath:ProfilePhotoPath"]);

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        // Check the allowed extenstions
        var ext = Path.GetExtension(imageFile.FileName);
        if (!allowedFileExtensions.Contains(ext))
            throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");

        // generate a unique filename
        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);
        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await imageFile.CopyToAsync(stream);
        return fileName;
    }

    public string GetProfilePhotoPath(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension)) throw new FileNotFoundException("Invalid filepath provided");

        var contentPath = _environment.ContentRootPath;
        var path = Path.Combine(contentPath, _configuration["StoragePath:ProfilePhotoPath"], fileNameWithExtension);

        if (!File.Exists(path)) throw new FileNotFoundException("Profile photo not found.");

        return path;
    }

    public async Task<string> ModifyProfilePhoto(IFormFile imageFile, string[] allowedFileExtensions,
        string existingFileName)
    {
        if (imageFile == null) throw new ArgumentNullException(nameof(imageFile));

        var contentPath = _environment.ContentRootPath;
        var path = Path.Combine(contentPath, _configuration["StoragePath:ProfilePhotoPath"]);

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        // Check the allowed extenstions
        var ext = Path.GetExtension(imageFile.FileName);
        if (!allowedFileExtensions.Contains(ext))
            throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");

        // generate a unique filename
        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);
        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        // Delete old image
        var response = DeleteProfilePhoto(existingFileName);

        return fileName;
    }

    public int DeleteProfilePhoto(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension)) throw new ArgumentNullException(nameof(fileNameWithExtension));
        var contentPath = _environment.ContentRootPath;
        var path = Path.Combine(contentPath, _configuration["StoragePath:ProfilePhotoPath"], fileNameWithExtension);

        if (!File.Exists(path)) throw new FileNotFoundException("Invalid file path");
        File.Delete(path);

        return 200;
    }

    public async Task<string> SavePostBanner(IFormFile imageFile, string[] allowedFileExtensions)
    {
        if (imageFile == null) throw new ArgumentNullException(nameof(imageFile));

        var contentPath = _environment.ContentRootPath;
        var path = Path.Combine(contentPath, _configuration["StoragePath:FrontBannerPhotoPath"]);

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        // Check the allowed extenstions
        var ext = Path.GetExtension(imageFile.FileName);
        if (!allowedFileExtensions.Contains(ext))
            throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");

        // generate a unique filename
        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);
        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await imageFile.CopyToAsync(stream);
        return fileName;
    }
    
    public async Task<string> ModifyPostBanner(IFormFile imageFile, string[] allowedFileExtensions,
        string existingFileName)
    {
        if (imageFile == null) throw new ArgumentNullException(nameof(imageFile));

        var contentPath = _environment.ContentRootPath;
        var path = Path.Combine(contentPath, _configuration["StoragePath:FrontBannerPhotoPath"]);

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        // Check the allowed extenstions
        var ext = Path.GetExtension(imageFile.FileName);
        if (!allowedFileExtensions.Contains(ext))
            throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");

        // generate a unique filename
        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);
        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        // Delete old image
        var response = DeleteFrontBanner(existingFileName);

        return fileName;
    }
    
    public string GetFrontBannerPath(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension)) throw new FileNotFoundException("Invalid filepath provided");

        var contentPath = _environment.ContentRootPath;
        var path = Path.Combine(contentPath, _configuration["StoragePath:FrontBannerPhotoPath"], fileNameWithExtension);

        if (!File.Exists(path)) throw new FileNotFoundException("Profile photo not found.");

        return path;
    }
    
    public int DeleteFrontBanner(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension)) throw new ArgumentNullException(nameof(fileNameWithExtension));
        var contentPath = _environment.ContentRootPath;
        var path = Path.Combine(contentPath, _configuration["StoragePath:FrontBannerPhotoPath"], fileNameWithExtension);

        if (!File.Exists(path)) throw new FileNotFoundException("Invalid file path");
        File.Delete(path);

        return 200;
    }
}