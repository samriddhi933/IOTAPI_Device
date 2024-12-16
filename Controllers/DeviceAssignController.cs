using IOTapi.dbcontext;
using IOTapi.Model;
using IOTapi.Services;
using IOTapi.Sevice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IOTapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceAssignController : ControllerBase
    {
        DeviceAssignService _deviceassign;
        public DeviceAssignController(ferrodbContext context)
        {
            _deviceassign = new DeviceAssignService(context);
        }
        [HttpGet]
        public IActionResult GetList()
        {
            var deviceassign = _deviceassign.GetAll().OrderByDescending(c => c.Id).ToList(); 
            if (deviceassign == null || !deviceassign.Any())
            {
                return NotFound("Component Not Found");
            }
            else
            {
                return Ok(deviceassign);
            }
        }
        [HttpGet("DeviceAssign/{entityId}")]
        public IActionResult GetByEntityId(int entityId)
        {
            try
            {
                // Validate parameter before using it
                if (entityId <= 0)
                {
                    return BadRequest("entityId must be a positive integer.");
                }

                // Call the service method that executes the parameterized query
                var deviceAssign = _deviceassign.GetDeviceAssignByEntityId(entityId);

                // Check if Assign are found
                if (deviceAssign == null || !deviceAssign.Any())
                {
                    return NotFound("Device assignments not found for the given entityId.");
                }

                return Ok(deviceAssign); // Return the dynamic result
            }
            catch (Exception ex)
            {
                // Return a generic internal server error message
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Create(DeviceAssign deviceAssign)
        {

            var addDevice = _deviceassign.AddDeviceAssign(deviceAssign);
            if (addDevice == "OK")
            {
                return Ok(addDevice);
            }
            else
            {
                return NotFound(addDevice);
            }
        }
        [HttpPut]
        public IActionResult Update(DeviceAssign deviceAssign)
        {
            var upDevice = _deviceassign.UpdateDevice(deviceAssign);
            if (upDevice == "OK")
            {
                return Ok(upDevice);
            }
            else
            {
                return NotFound(upDevice);
            }
        }
        [HttpGet("unassigned")]
        public ActionResult<IEnumerable<dynamic>> GetUnassignedDevices()
        {
            var devices = _deviceassign.GetUnassignedDevices();  // Call the service method
            return Ok(devices);  // Return the dynamic result
        }

    }
}
