using IOTapi.dbcontext;
using IOTapi.Model;
using Microsoft.EntityFrameworkCore;

namespace IOTapi.Sevice
{
    public class DeviceDetailService
    {
        private readonly ferrodbContext _context;
        public DeviceDetailService(ferrodbContext ferrodbContext)
        {
            _context = ferrodbContext;
        }
        public List<DeviceDetail> GetAll()
        {
            try
            {
                return _context.DeviceDetails.OrderByDescending(c => c.Id).ToList(); // Sorting in descending order
            }
            catch
            {
                return new List<DeviceDetail>();
            }
        }
        public (List<DeviceDetail> deviceDetails, int TotalCount) GetAll(int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                var query = _context.DeviceDetails.AsQueryable();

                // Get the total count of devices
                var totalCount = query.Count();

                // Apply pagination if parameters are provided
                if (pageNumber.HasValue && pageSize.HasValue)
                {
                    query = query
                        .Skip((pageNumber.Value - 1) * pageSize.Value)
                        .Take(pageSize.Value);
                }

                // Return the paginated result and the total count
                return (query.ToList(), totalCount);
            }
            catch
            {
                return (new List<DeviceDetail>(), 0);
            }
        }

        public DeviceDetail? GetDeviceById(int Id)
        {
            try
            {
                return _context.DeviceDetails.Find(Id);
            }
            catch
            {
                return null;
            }
        }


        public string AddDevice(DeviceDetail devicedetails)
        {
            try
            {
                var Adddevice = new DeviceDetail()
                {
                    Id = devicedetails.Id,
                    DTypeId = devicedetails.DTypeId,
                    AttributeId = devicedetails.AttributeId,
                };
                _context.DeviceDetails.Add(Adddevice);
                _context.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string UpdateDevice(DeviceDetail Devices)
        {
            try
            {
                var Editdevice = _context.DeviceDetails.Find(Devices.Id);

                Editdevice.Id = Devices.Id;
                Editdevice.DTypeId = Devices.DTypeId;
                Editdevice.AttributeId = Devices.AttributeId;
                _context.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string DeleteDevice(int Id)
        {
            try
            {
                var deletedevice = _context.DeviceDetails.Find(Id);
                _context.DeviceDetails.Remove(deletedevice);
                _context.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
