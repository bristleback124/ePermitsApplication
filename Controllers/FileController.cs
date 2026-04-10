using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;

    public FileController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFile([FromQuery] string path, [FromQuery] bool download = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            return BadRequest("Invalid path");

        Stream stream;
        try
        {
            stream = await _fileStorageService.DownloadAsync(path, cancellationToken);
        }
        catch (FileNotFoundException)
        {
            return NotFound("File not found");
        }

        var downloadName = Path.GetFileName(path);
        var mimeType = GetMimeType(downloadName);

        if (download)
        {
            return File(stream, mimeType, fileDownloadName: downloadName);
        }

        return File(stream, mimeType);
    }

    private string GetMimeType(string filePath) => Path.GetExtension(filePath).ToLowerInvariant() switch
    {
        ".pdf"  => "application/pdf",
        ".png"  => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".doc"  => "application/msword",
        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        _       => "application/octet-stream"
    };
}
