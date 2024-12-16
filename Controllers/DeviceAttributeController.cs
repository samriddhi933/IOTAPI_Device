using IOTapi.dbcontext;
using IOTapi.Model;
using IOTapi.Sevice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IOTapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceAttributeController : ControllerBase
    {
        DeviceAttributeService _deviceAttribute;
        public DeviceAttributeController(ferrodbContext context)
        {
            _deviceAttribute = new DeviceAttributeService(context);
        }
        [HttpGet]
        public IActionResult GetList()
        {
            var deviceAttributes = _deviceAttribute.GetAll().OrderByDescending(c => c.Id).ToList(); // Replace 'Id' with your desired field for sorting
            if (deviceAttributes == null || !deviceAttributes.Any())
            {
                return NotFound("Component Not Found");
            }
            else
            {
                return Ok(deviceAttributes);
            }
        }
        [HttpGet("attributes/{deviceId}")]
        public IActionResult GetByDeviceId(int deviceId)
        {
            try
            {
                // Validate input
                if (deviceId <= 0)
                {
                    return BadRequest("Invalid Device ID. Device ID must be a positive integer.");
                }

                // Call the service method to fetch attributes
                var attributes = _deviceAttribute.GetDeviceattributeById(deviceId);

                // Check if attributes are found
                if (attributes == null || !attributes.Any())
                {
                    return NotFound($"No attributes found for Device ID {deviceId}.");
                }

                return Ok(attributes); // Return the result
            }
            catch (Exception ex)
            {
                // Log the exception (add your logging mechanism here)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Create(List<DeviceAttribute> deviceAttributes, [FromQuery] int deviceId)
        {
            // Call the AddDeviceAttributes method, passing the list of deviceAttributes and deviceId
            var addAttributes = _deviceAttribute.AddDeviceAttributes(deviceAttributes, deviceId);

            if (addAttributes == "OK")
            {
                return Ok("Device attributes created and linked to device successfully.");
            }
            else
            {
                return BadRequest(addAttributes);
            }

        }
        [HttpPut]
        public IActionResult Update(DeviceAttribute deviceAttribute)
        {
            var upattribute = _deviceAttribute.UpdateDevice(deviceAttribute);
            if (upattribute == "OK")
            {
                return Ok(upattribute);
            }
            else
            {
                return NotFound(upattribute);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteDeviceAttribute(int deviceId, int attributeId)
        {
            // Call the service to delete the association and attribute
            var deleteResult = await _deviceAttribute.DeleteDeviceAttribute(deviceId, attributeId);

            if (deleteResult)
            {
                return Ok(new { message = "Device attribute deleted successfully.", success = true });
            }

            return BadRequest(new { message = "Failed to delete device attribute. Either the association or attribute does not exist.", success = false });
        }

    }
}
