using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IOTapi.dbcontext;
using IOTapi.Model;
using IOTapi.Sevice;

namespace IOTapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentController : ControllerBase
    {
        ComponentService _componentService;
        public ComponentController(ferrodbContext component)
        {
            _componentService = new ComponentService(component);
        }
        [HttpGet]
        public IActionResult GetList()
        {
            var component = _componentService.GetAll().OrderByDescending(c => c.Id).ToList();
            if (component == null || !component.Any())
            {
                return NotFound("Component Not Found");
            }
            else
            {
                return Ok(component);
            }
        }

        [HttpGet("Component/{deviceId}")]
        public IActionResult GetByDeviceId(int deviceId)
        {
            try
            {
                // Validate input
                if (deviceId <= 0)
                {
                    return BadRequest("Invalid Device ID. Device ID must be a positive integer.");
                }

                // Call the service method to fetch component
                var attributes = _componentService.GetDeviceattributeById(deviceId);

                // Check if component are found
                if (attributes == null || !attributes.Any())
                {
                    return NotFound($"No component found for Device ID {deviceId}.");
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
        public IActionResult Create(List<Component> components, [FromQuery] int MasterId)
        {
            // Call the AddDevicecomponent method, passing the list of devicecomponent and deviceId
            var component = _componentService.Addcomponents(components, MasterId);

            if (component == "OK")
            {
                return Ok("Component created and linked to Component Device successfully.");
            }
            else
            {
                return BadRequest(component);
            }
        }

        [HttpPut]
        public IActionResult Update(Component component)
        {
            var upcomponents = _componentService.Updatecomponent(component);
            if (upcomponents == "OK")
            {
                return Ok(upcomponents);
            }
            else
            {
                return NotFound(upcomponents);
            }
        }
        [HttpDelete("{Id}")]
        public IActionResult Delete(int Id)
        {
            var deletecomponent = _componentService.Deletecomponent(Id);
            if (deletecomponent == "OK")
            {
                return Ok(deletecomponent);
            }
            else
            {
                return NotFound(deletecomponent);
            }
        }
    }

   
}
