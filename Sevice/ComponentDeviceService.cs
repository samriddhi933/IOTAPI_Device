using IOTapi.dbcontext;
using IOTapi.Model;

namespace IOTapi.Sevice
{
    public class ComponentDeviceService
    {
        private readonly ferrodbContext _context;
        public ComponentDeviceService(ferrodbContext context)
        {
            _context = context;
        }
        public List<ComponentDevice> GetAll()
        {
            try
            {
                return _context.ComponentDevices.OrderByDescending(c => c.Id).ToList(); // Sorting in descending order
            }
            catch
            {
                return new List<ComponentDevice>();
            }
        }

        public ComponentDevice? GetcomponentById(int Id)
        {
            try
            {
                return _context.ComponentDevices.Find(Id);
            }
            catch
            {
                return null;
            }
        }

        public string AddComponent(ComponentDevice componentdevice)
        {
            try
            {
                var Addcomponent = new ComponentDevice()
                {
                    Id = componentdevice.Id,
                    ComponentId = componentdevice.ComponentId,
                    MasterId = componentdevice.MasterId,
                    Installdate = DateTime.Now,
                };
                _context.ComponentDevices.Add(Addcomponent);
                _context.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string Updatecomponent(ComponentDevice componentDevice)
        {
            try
            {
                var Editcomponet = _context.ComponentDevices.Find(componentDevice.Id);

                Editcomponet.Id = componentDevice.Id;
                Editcomponet.ComponentId = componentDevice.ComponentId;
                Editcomponet.MasterId = componentDevice.MasterId;
                Editcomponet.Installdate = componentDevice.Installdate;

                _context.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string Deletecomponent(int Id)
        {
            try
            {
                var deletecomponent = _context.ComponentDevices.Find(Id);
                _context.ComponentDevices.Remove(deletecomponent);
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
