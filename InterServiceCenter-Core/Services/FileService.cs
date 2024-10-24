namespace InterServiceCenter_Core.Services;

public interface IFileService
{

}

public class FileService(IWebHostEnvironment environment) : IFileService
{
    public async Task<string> SaveProfilePhoto(IFormFile imageFile, string[] allowedFileExtensions)
    {
        if (imageFile == null)
        {
            throw new ArgumentNullException(nameof(imageFile));
        }
        
        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, "Uploads/ProfilePhotos");
        
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        // Check the allowed extenstions
        var ext = Path.GetExtension(imageFile.FileName);
        if (!allowedFileExtensions.Contains(ext))
        {
            throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");
        }
        
        // generate a unique filename
        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);
        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await imageFile.CopyToAsync(stream);
        return fileName;
    }
    
    public async Task<string> SavePostBanner(IFormFile imageFile, string[] allowedFileExtensions)
    {
        if (imageFile == null)
        {
            throw new ArgumentNullException(nameof(imageFile));
        }
        
        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, "Uploads/PostBanner");
        
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        // Check the allowed extenstions
        var ext = Path.GetExtension(imageFile.FileName);
        if (!allowedFileExtensions.Contains(ext))
        {
            throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");
        }
        
        // generate a unique filename
        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);
        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await imageFile.CopyToAsync(stream);
        return fileName;
    }
    
    public int DeleteProfilePhoto(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension))
        {
            throw new ArgumentNullException(nameof(fileNameWithExtension));
        }
        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, $"Uploads/ProfilePhotos", fileNameWithExtension);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Invalid file path");
        }
        File.Delete(path);

        return 200;
    }
    
    public void DeletePostBanner(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension))
        {
            throw new ArgumentNullException(nameof(fileNameWithExtension));
        }
        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, $"Uploads/PostBanner", fileNameWithExtension);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Invalid file path");
        }
        File.Delete(path);
    }
}