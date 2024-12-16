using IOTapi.dbcontext;
using IOTapi.Model;
using IOTapi.Sevice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IOTapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDeviceTypeController : ControllerBase
    {
        MasterDeviceTypeService _masterDeviceType;
        public MasterDeviceTypeController(ferrodbContext context)
        {
            _masterDeviceType = new MasterDeviceTypeService(context);
        }
        [HttpGet]
        public IActionResult GetList()
        {
            var deviceTypes = _masterDeviceType.GetAll().OrderByDescending(c => c.Id).ToList(); 
            if (deviceTypes == null || !deviceTypes.Any())
            {
                return NotFound("Component Not Found");
            }
            else
            {
                return Ok(deviceTypes);
            }
        }
        [HttpGet("{Id}")]
        public IActionResult Getbyid(int Id)
        {
            var getid = _masterDeviceType.GetDeviceMasterById(Id);
            if (getid == null)
            {
                return NotFound("MasterDevice not found");
            }
            else
            {
                return Ok(getid);
            }
        }
        [HttpPost]
        public IActionResult Create(MasterDeviceType masterDevice)
        {

            var addDevice = _masterDeviceType.AddDeviceMaster(masterDevice);
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
        public IActionResult Update(MasterDeviceType masterDevice)
        {
            var upDevice = _masterDeviceType.UpdateDeviceMaster(masterDevice);
            if (upDevice == "OK")
            {
                return Ok(upDevice);
            }
            else
            {
                return NotFound(upDevice);
            }
        }
        [HttpDelete("{Id}")]
        public IActionResult Delete(int Id)
        {
            var deleteDevice = _masterDeviceType.DeleteDeviceMaster(Id);
            if (deleteDevice == "OK")
            {
                return Ok(deleteDevice);
            }
            else
            {
                return NotFound(deleteDevice);
            }
        }

    }
}
