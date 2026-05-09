using Gateway.Models;
using Gateway.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Gateway.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class CamerasController(IOptions<CameraInputOptions> options) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<CameraDto>> List()
    {
        var rootPath = options.Value.RootPath;

        if (!Directory.Exists(rootPath))
        {
            return Ok(Array.Empty<CameraDto>());
        }

        var cameras = new List<CameraDto>();

        foreach (var path in Directory.EnumerateFiles(rootPath, "*.mp4"))
        {
            var fileName = Path.GetFileNameWithoutExtension(path);

            if (!Guid.TryParse(fileName, out var cameraId))
            {
                continue;
            }

            var info = new FileInfo(path);

            cameras.Add(new CameraDto
            {
                Id = cameraId,
                DisplayName = $"Camera {cameraId:D}",
                FileSize = info.Length
            });
        }

        return Ok(cameras);
    }
}
