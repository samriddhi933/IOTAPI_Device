using IOTapi.dbcontext;
using IOTapi.Model;
using IOTapi.Services;
using IOTapi.Sevice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IOTapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceInstallationController : ControllerBase
    {
        private readonly DeviceInstallationService _service;
        private readonly ILogger<DeviceInstallationController> _logger;

        public DeviceInstallationController(DeviceInstallationService service, ILogger<DeviceInstallationController> logger)
        {
            _service = service;
            _logger = logger;
        }
        [HttpPost("SaveInstallationWithImages")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SaveInstallationWithImages([FromForm] string deviceDetails, [FromForm] List<IFormFile> imgData)
        {
            try
            {
                _logger.LogInformation("Received request to save installation.");

                // Validate deviceDetails
                if (string.IsNullOrEmpty(deviceDetails))
                {
                    return BadRequest(new { msg = "Device details are required." });
                }

                // Deserialize deviceDetails
                   Deviceinstallation? installationData;
                try
                {
                    installationData = JsonConvert.DeserializeObject<Deviceinstallation>(deviceDetails);
                    if (installationData == null)
                    {
                        throw new JsonException("Deserialization returned null.");
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Failed to deserialize deviceDetails: {ex.Message}");
                    return BadRequest(new { msg = "Invalid JSON format for deviceDetails." });
                }

                // Validate image formats
                if (imgData.Any(image => !_service.ValidateImageFormat(image)))
                {
                    return BadRequest(new { msg = "One or more images have an invalid format or exceed size limits." });
                }

                // Save installation and images
                var (isSuccess, message) = await _service.SaveDeviceInstallationAsync(installationData, imgData);

                if (isSuccess)
                {
                    return Ok(new { msg = "Installation and images saved successfully." });
                }
                else
                {
                    _logger.LogError($"Service failure: {message}");
                    return StatusCode(500, new { msg = message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred: {ex}");
                return StatusCode(500, new { msg = "An unexpected error occurred.", details = ex.Message });
            }


        }
        [HttpGet("DeviceInstallation/{roleId}/{entityId}")]
        public IActionResult GetByDeviceId(int roleId, int entityId)
        {
            try
            {
                if (roleId <= 0 || entityId <= 0)
                {
                    return BadRequest("Both roleId and entityId must be positive integers.");
                }

                var deviceAssign = _service.GetDeviceAggignById(roleId, entityId);

                if (deviceAssign == null || !deviceAssign.Any())
                {
                    return NotFound("Device installation not found for the given roleId and entityId.");
                }

                return Ok(deviceAssign);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }
    }
}
