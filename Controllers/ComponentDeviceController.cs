using IOTapi.dbcontext;
using IOTapi.Model;
using IOTapi.Sevice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IOTapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentDeviceController : ControllerBase
    {
        ComponentDeviceService _componentDevice;
        public ComponentDeviceController(ferrodbContext context)
        {
            _componentDevice = new ComponentDeviceService(context);
        }
        [HttpGet]
        public IActionResult GetList()
        {
            var component = _componentDevice.GetAll().OrderByDescending(c => c.Id).ToList(); 
            if (component == null || !component.Any())
            {
                return NotFound("Component Not Found");
            }
            else
            {
                return Ok(component);
            }
        }

        [HttpGet("{Id}")]
        public IActionResult Getbyid(int Id)
        {
            var getid = _componentDevice.GetcomponentById(Id);
            if (getid == null)
            {
                return NotFound("Component not found");
            }
            else
            {
                return Ok(getid);
            }
        }
        [HttpPost]
        public IActionResult Create(ComponentDevice component)
        {

            var addcomponent = _componentDevice.AddComponent(component);
            if (addcomponent == "OK")
            {
                return Ok(addcomponent);
            }
            else
            {
                return NotFound(addcomponent);
            }
        }

        [HttpPut]
        public IActionResult Update(ComponentDevice component)
        {
            var upcomponents = _componentDevice.Updatecomponent(component);
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
            var deletecomponent = _componentDevice.Deletecomponent(Id);
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
