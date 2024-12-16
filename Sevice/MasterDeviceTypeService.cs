using IOTapi.dbcontext;
using IOTapi.Model;

namespace IOTapi.Sevice
{
    public class MasterDeviceTypeService
    {
        private readonly ferrodbContext _context;
        public MasterDeviceTypeService(ferrodbContext context)
        {
            _context = context;
        }
        public List<MasterDeviceType> GetAll()
        {
            try
            {
                return _context.MasterDeviceTypes.OrderByDescending(c => c.Id).ToList(); // Sorting in descending order
            }
            catch
            {
                return new List<MasterDeviceType>();
            }
        }
        public MasterDeviceType? GetDeviceMasterById(int Id)
        {
            try
            {
                return _context.MasterDeviceTypes.Find(Id);
            }
            catch
            {
                return null;
            }
        }

        public string AddDeviceMaster(MasterDeviceType masterDeviceType)
        {
            try
            {
                var Adddevicemaster = new MasterDeviceType()
                {
                    Id = masterDeviceType.Id,
                    DeviceName = masterDeviceType.DeviceName,
                    Make = masterDeviceType.Make,
                    Model = masterDeviceType.Model,
                    Remarks = masterDeviceType.Remarks,
                };
                _context.MasterDeviceTypes.Add(Adddevicemaster);
                _context.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string UpdateDeviceMaster(MasterDeviceType masterDevice)
        {
            try
            {
                var EditdeviceMaster = _context.MasterDeviceTypes.Find(masterDevice.Id);

                EditdeviceMaster.Id = masterDevice.Id;
                EditdeviceMaster.DeviceName = masterDevice.DeviceName;
                EditdeviceMaster.Make = masterDevice.Make;
                EditdeviceMaster.Model = masterDevice.Model;
                EditdeviceMaster.Remarks = masterDevice.Remarks;

                _context.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string DeleteDeviceMaster(int Id)
        {
            try
            {
                var deletedeviceMaster = _context.MasterDeviceTypes.Find(Id);
                _context.MasterDeviceTypes.Remove(deletedeviceMaster);
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
