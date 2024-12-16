using IOTapi.dbcontext;
using IOTapi.Model;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Security.Cryptography;

namespace IOTapi.Sevice
{
    public class DeviceAssignService
    {
        private readonly ferrodbContext _context;
        public DeviceAssignService(ferrodbContext context)
        {
            _context = context;
        }
        public List<DeviceAssign> GetAll()
        {
            try
            {
                return _context.DeviceAssigns.OrderByDescending(c => c.Id).ToList(); // Sorting in descending order
            }
            catch
            {
                return new List<DeviceAssign>();
            }
        }
        public IEnumerable<dynamic> GetDeviceAssignByEntityId(int entityId)
        {
            var query = @"
    SELECT 
        da.deviceid, 
        d.imie, 
        da.installationdatetime, 
        CONCAT(a.addressline1, ' ', a.addressline2) AS address, 
        CONCAT(p.firstname, ' ', p.lastname) AS person, 
        e.id, 
        e.Name, 
        da.remarks 
    FROM 
        ferrodb.device_assign da
    LEFT JOIN 
        device d ON da.deviceid = d.id
    LEFT JOIN 
        address a ON a.id = da.AddressId
    LEFT JOIN 
        person p ON p.id = da.approverpersonid
    LEFT JOIN 
        entity e ON e.id = da.EntityId
    WHERE 
        da.EntityId = @entityId;
    ";

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;

                // Add the parameter for entityId
                var entityIdPara = command.CreateParameter();
                entityIdPara.ParameterName = "@entityId";
                entityIdPara.Value = entityId;
                command.Parameters.Add(entityIdPara);

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
                    throw new Exception($"Error executing query for entityId {entityId}: {ex.ToString()}", ex);
                }
                finally
                {
                    _context.Database.CloseConnection();
                }
            }
        }

        public string AddDeviceAssign(DeviceAssign deviceAssign)
        {
            try
            {
                var Adddeviceassign = new DeviceAssign()
                {
                    Id = deviceAssign.Id,
                    Deviceid = deviceAssign.Deviceid,
                    Installationdatetime = deviceAssign.Installationdatetime,
                    AddressId = deviceAssign.AddressId,
                    Approverpersonid = deviceAssign.Approverpersonid,
                    EntityId = deviceAssign.EntityId,
                    Remarks = deviceAssign.Remarks,
                };
                _context.DeviceAssigns.Add(Adddeviceassign);
                _context.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string UpdateDevice(DeviceAssign DeviceAssign)
        {
            try
            {
                var Editdevice = _context.DeviceAssigns.Find(DeviceAssign.Id);

                Editdevice.Id = DeviceAssign.Id;
                Editdevice.Deviceid = DeviceAssign.Deviceid;
                Editdevice.Installationdatetime = DeviceAssign.Installationdatetime;
                Editdevice.AddressId = DeviceAssign.AddressId;
                Editdevice.Approverpersonid = DeviceAssign.Approverpersonid;
                Editdevice.EntityId = DeviceAssign.EntityId;
                Editdevice.Remarks = DeviceAssign.Remarks;

                _context.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public IEnumerable<dynamic> GetUnassignedDevices()
        {
            // Raw SQL query as a regular string (no parameterized query)
            var query = @"
                  SELECT d.*, d.devicetypeid
                  FROM device_assign
                  RIGHT JOIN (
                      SELECT device.id, device.imie, mdt.device_name, device.devicetypeid
                      FROM device
                      JOIN master_device_type AS mdt ON device.devicetypeid = mdt.id
                  ) AS d 
                  ON d.id = device_assign.deviceid
                  WHERE device_assign.id IS NULL;
                  ";

            // Execute the raw SQL query using ExecuteSqlRaw
            var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
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

            // Sort the result by a specific field, e.g., devicetypeid in descending order
            return result.OrderByDescending(device => (device as IDictionary<string, object>)["devicetypeid"]);
        }

    }
}
