
using HealthPlus.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthPlus.API.Controllers;

[ApiController]
[Route("api/upload")]
[Authorize(Roles = "Admin")]
public class UploadController : ControllerBase
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    private readonly IWebHostEnvironment _environment;

    public UploadController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("product-image")]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<IActionResult> UploadProductImage(
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(
                ApiResponse<string>.Fail("Vui lòng chọn ảnh."));
        }

        if (file.Length > MaxFileSize)
        {
            return BadRequest(
                ApiResponse<string>.Fail("Ảnh không được vượt quá 5 MB."));
        }

        var extension = GetImageExtension(file);

        if (extension is null)
        {
            return BadRequest(
                ApiResponse<string>.Fail(
                    "Chỉ chấp nhận ảnh JPG, PNG hoặc WebP hợp lệ."));
        }

        var uploadDirectory = Path.Combine(
            _environment.WebRootPath
                ?? Path.Combine(_environment.ContentRootPath, "wwwroot"),
            "uploads",
            "products");

        Directory.CreateDirectory(uploadDirectory);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadDirectory, fileName);

        try
        {
            await using var stream = new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true);

            await file.CopyToAsync(stream, cancellationToken);

            var imageUrl =
                $"{Request.Scheme}://{Request.Host}/uploads/products/{fileName}";

            return Ok(
                ApiResponse<string>.Ok(imageUrl, "Upload ảnh thành công."));
        }
        catch (OperationCanceledException)
        {
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            throw;
        }
        catch (IOException)
        {
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<string>.Fail("Không thể lưu ảnh lên máy chủ."));
        }
    }

    private static string? GetImageExtension(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName)
            .ToLowerInvariant();

        if (extension is not ".jpg" and not ".jpeg"
            and not ".png" and not ".webp")
        {
            return null;
        }

        Span<byte> header = stackalloc byte[12];

        using var stream = file.OpenReadStream();
        var bytesRead = stream.Read(header);

        // JPEG: FF D8 FF
        if (bytesRead >= 3
            && header[0] == 0xFF
            && header[1] == 0xD8
            && header[2] == 0xFF
            && extension is ".jpg" or ".jpeg")
        {
            return ".jpg";
        }

        // PNG signature
        if (bytesRead >= 8
            && header[0] == 0x89
            && header[1] == 0x50
            && header[2] == 0x4E
            && header[3] == 0x47
            && header[4] == 0x0D
            && header[5] == 0x0A
            && header[6] == 0x1A
            && header[7] == 0x0A
            && extension == ".png")
        {
            return ".png";
        }

        // WebP: RIFF....WEBP
        if (bytesRead >= 12
            && header[0] == (byte)'R'
            && header[1] == (byte)'I'
            && header[2] == (byte)'F'
            && header[3] == (byte)'F'
            && header[8] == (byte)'W'
            && header[9] == (byte)'E'
            && header[10] == (byte)'B'
            && header[11] == (byte)'P'
            && extension == ".webp")
        {
            return ".webp";
        }

        return null;
    }
}
