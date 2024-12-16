using IOTapi.dbcontext;
using IOTapi.Model;
 using MySql.Data.MySqlClient;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Dynamic;

namespace IOTapi.Services
{
    public class DeviceService
    {
        private readonly ferrodbContext _context;

        // Constructor to inject the DbContext
        public DeviceService(ferrodbContext context)
        {
            _context = context;
        }

        public List<Device> GetAll()
        {
            try
            {
                return _context.Devices.OrderByDescending(c => c.Id).ToList(); // Sorting in descending order
            }
            catch
            {
                return new List<Device>();
            }
        }


        public Device? GetDeviceById(int Id)
        {
            try
            {
                return _context.Devices.Find(Id);
            }
            catch
            {
                return null;
            }
        }
        public string AddDevice(Device devices)
        {
            try
            {
                var Devices = new Device()
                {
                    Id = devices.Id,
                    Imie = devices.Imie,
                    Devicetypeid = devices.Devicetypeid,
                    Parentid = devices.Parentid

                };
                _context.Devices.Add(Devices);
                _context.SaveChanges(); // Save the new attribute                                 

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string Updatedevice(Device device)
        {
            try
            {
                var Editdevice = _context.Devices.Find(device.Id);

                Editdevice.Id = device.Id;
                Editdevice.Imie = device.Imie;
                Editdevice.Devicetypeid = device.Devicetypeid;  
                Editdevice.Parentid = device.Parentid;

                _context.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string Deletedevices(int Id)
        {
            try
            {
                var deletedevice = _context.Devices.Find(Id);
                _context.Devices.Remove(deletedevice);
                _context.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public IEnumerable<dynamic> GetAllDeviceTypes()
        {
            // Query without filtering by deviceId
            var query = @"SELECT d.id deviceId, d.imie, mdt.* 
                  FROM device d
                  LEFT JOIN master_device_type mdt ON d.devicetypeid = mdt.id";

            // Open connection to the database
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;

                try
                {
                    _context.Database.OpenConnection();

                    var result = new List<dynamic>();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dynamic expando = new ExpandoObject();
                            var expandoDict = (IDictionary<string, object>)expando;

                            // Loop through the columns and add to ExpandoObject
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                expandoDict[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }

                            result.Add(expando);
                        }
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    // Log the exception if needed
                    throw new Exception($"Error executing query: {ex.Message}", ex);
                }
                finally
                {
                    // Ensure that the connection is always closed
                    _context.Database.CloseConnection();
                }
            }
        }

    }
}