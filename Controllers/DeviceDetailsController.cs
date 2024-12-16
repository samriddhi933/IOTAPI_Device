using IOTapi.dbcontext;
using IOTapi.Model;
using IOTapi.Sevice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IOTapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceDetailsController : ControllerBase
    {
        DeviceDetailService _detailService;
        public DeviceDetailsController(ferrodbContext context)
        {
            _detailService = new DeviceDetailService(context);
        }
        [HttpGet]
        public IActionResult GetList()
        {
            var deviceDetails = _detailService.GetAll().OrderByDescending(c => c.Id).ToList();
            if (deviceDetails == null || !deviceDetails.Any())
            {
                return NotFound("Component Not Found");
            }
            else
            {
                return Ok(deviceDetails);
            }
        }
        [HttpGet("pagination")]
        public IActionResult GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            // Check for valid pagination values
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }

            // Get paginated devices from the service
            var (devices, totalCount) = _detailService.GetAll(pageNumber, pageSize);

            // Check if devices exist
            if (!devices.Any())
            {
                return NotFound("Devices not found.");
            }

            // Construct a response object including pagination metadata
            var response = new
            {
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Data = devices
            };

            return Ok(response);
        }


        [HttpGet("{Id}")]
        public IActionResult Getbyid(int Id)
        {
            var getid = _detailService.GetDeviceById(Id);
            if (getid == null)
            {
                return NotFound("DeviceDetails not found");
            }
            else
            {
                return Ok(getid);
            }
        }
        [HttpPost]
        public IActionResult Create(DeviceDetail deviceDetail)
        {

            var addDevice = _detailService.AddDevice(deviceDetail);
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
        public IActionResult Update(DeviceDetail deviceDetail)
        {
            var upDevice = _detailService.UpdateDevice(deviceDetail);
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
            var deleteDevice = _detailService.DeleteDevice(Id);
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
