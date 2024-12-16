
using IOTapi.Services;
using IOTapi.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using IOTapi.dbcontext;
using IOTapi.Sevice;

namespace DeviceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        DeviceService _deviceService;
        public DeviceController(ferrodbContext context)
        {
            _deviceService = new DeviceService(context);
        }
        [HttpGet]
        public IActionResult GetList()
        {
            var device = _deviceService.GetAll().OrderByDescending(c => c.Id).ToList();
            if (device == null || !device.Any())
            {
                return NotFound("Device Not Found");
            }
            else
            {
                return Ok(device);
            }
        }

        [HttpGet("{Id}")]
        public IActionResult Getbyid(int Id)
        {
            var getid = _deviceService.GetDeviceById(Id);
            if (getid == null)
            {
                return NotFound("device not found");
            }
            else
            {
                return Ok(getid);
            }
        }
        [HttpPost]
        public IActionResult Create(Device device)
        {

            var adddevice = _deviceService.AddDevice(device);
            if (adddevice == "OK")
            {
                return Ok(adddevice);
            }
            else
            {
                return NotFound(adddevice);
            }
        }
        [HttpPut]
        public IActionResult Update(Device device)
        {
            var upcomponents = _deviceService.Updatedevice(device);
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
            var deletecomponent = _deviceService.Deletedevices(Id);
            if (deletecomponent == "OK")
            {
                return Ok(deletecomponent);
            }
            else
            {
                return NotFound(deletecomponent);
            }
        }
        [HttpGet("Devices")]
        public IActionResult GetAllDevices()
        {
            try
            {
                // Call the service method to get all devices
                var devices = _deviceService.GetAllDeviceTypes();

                // Check if devices are found
                if (devices == null || !devices.Any())
                {
                    return NotFound("No devices found.");
                }

                return Ok(devices); // Return the dynamic result
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 