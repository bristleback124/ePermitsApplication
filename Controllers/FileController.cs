using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public FileController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetFile([FromQuery] string path, [FromQuery] bool download = false)
    {
        var basePath = _configuration["FileStorage:BasePath"];

        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(basePath))
            return BadRequest("Invalid path");

        var fullPath = Path.GetFullPath(path);
        var fullBasePath = Path.GetFullPath(basePath);

        // Case-insensitive comparison for Windows paths
        if (!fullPath.StartsWith(fullBasePath, StringComparison.OrdinalIgnoreCase))
            return BadRequest("Invalid path");

        if (!System.IO.File.Exists(fullPath))
            return NotFound("File not found");

        var mimeType = GetMimeType(fullPath);

        if (download)
        {
            return PhysicalFile(
                fullPath,
                mimeType,
                fileDownloadName: Path.GetFileName(fullPath),
                enableRangeProcessing: true);
        }

        return PhysicalFile(fullPath, mimeType, enableRangeProcessing: true);
    }

    private string GetMimeType(string filePath) => Path.GetExtension(filePath).ToLower() switch
    {
        ".pdf"  => "application/pdf",
        ".png"  => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".doc"  => "application/msword",
        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        _       => "application/octet-stream"
    };
}
