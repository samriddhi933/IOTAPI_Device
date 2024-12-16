using IOTapi.dbcontext;
using IOTapi.Model;
using IOTapi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Security.AccessControl;

namespace IOTapi.Sevice
{
    public class DeviceAttributeService
    {
        private readonly ferrodbContext _context;
        public DeviceAttributeService(ferrodbContext context)
        {
            _context = context;
        }
        public List<DeviceAttribute> GetAll()
        {
            try
            {
                return _context.DeviceAttributes.OrderByDescending(c => c.Id).ToList(); // Sorting in descending order
            }
            catch
            {
                return new List<DeviceAttribute>();
            }
        }      
        public IEnumerable<dynamic> GetDeviceattributeById(int deviceId)
        {
            // Validate the deviceId parameter
            if (deviceId <= 0)
            {
                throw new ArgumentException("Device ID must be a positive integer.", nameof(deviceId));
            }

            string rawSql = @"
        SELECT mdt.id AS MasterDeviceTypeId, da.* 
        FROM device_details dd
        LEFT JOIN master_device_type mdt ON dd.d_type_id = mdt.id
        LEFT JOIN device_attributes da ON dd.attribute_id = da.id
        WHERE mdt.id = @deviceId;
    ";

            // Open connection to the database
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = rawSql;
                command.CommandType = System.Data.CommandType.Text;

                // Add the parameter for deviceId
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@deviceId";
                parameter.Value = deviceId;
                command.Parameters.Add(parameter);

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
                    throw new Exception($"Error executing query for deviceId {deviceId}: {ex.Message}", ex);
                }
                finally
                {
                    // Ensure that the connection is always closed
                    _context.Database.CloseConnection();
                }
            }
        }

        public DeviceAttribute? GetDeviceById(int Id)
        {
            try
            {
                return _context.DeviceAttributes.Find(Id);
            }
            catch
            {
                return null;
            }
        }
        public string AddDeviceAttributes(List<DeviceAttribute> deviceAttributes, int deviceId)
        {
            try
            {
                foreach (var deviceAttribute in deviceAttributes)
                {
                    // Save the device attribute
                    var addAttribute = new DeviceAttribute()
                    {
                        Id = deviceAttribute.Id,
                        Name = deviceAttribute.Name,
                        Unit = deviceAttribute.Unit,
                        MinValue = deviceAttribute.MinValue,
                        MaxValue = deviceAttribute.MaxValue,
                        StandardValue = deviceAttribute.StandardValue,
                        Remarks = deviceAttribute.Remarks,
                    };
                    _context.DeviceAttributes.Add(addAttribute);
                    _context.SaveChanges(); // Save the new attribute

                    // Create and add to the device_details table
                    var deviceDetail = new DeviceDetail()
                    {
                        DTypeId = deviceId,  // The device ID passed into the method
                        AttributeId = addAttribute.Id  // The ID of the newly saved device attribute
                    };
                    _context.DeviceDetails.Add(deviceDetail);
                    _context.SaveChanges(); // Save the device detail record
                }

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string UpdateDevice(DeviceAttribute Device)
        {
            try
            {
                var EditAttribute = _context.DeviceAttributes.Find(Device.Id);

                EditAttribute.Id = Device.Id;
                EditAttribute.Name = Device.Name;
                EditAttribute.Unit = Device.Unit;
                EditAttribute.MinValue = Device.MinValue;
                EditAttribute.MaxValue = Device.MaxValue;
                EditAttribute.StandardValue = Device.StandardValue;
                EditAttribute.Remarks = Device.Remarks;

                _context.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public async Task<bool> DeleteDeviceAttribute(int deviceId, int attributeId)
        {
            // Find the association between device and attribute
            var deviceDetail = await _context.DeviceDetails
                .FirstOrDefaultAsync(dd => dd.DTypeId == deviceId && dd.AttributeId == attributeId);

            if (deviceDetail != null)
            {
                _context.DeviceDetails.Remove(deviceDetail); // Remove association
                await _context.SaveChangesAsync();
            }
            else
            {
                return false; // Association not found
            }

            // Delete the attribute if it exists
            var attribute = await _context.DeviceAttributes.FindAsync(attributeId);
            if (attribute != null)
            {
                _context.DeviceAttributes.Remove(attribute);
                await _context.SaveChangesAsync();
                return true;
            }

            return false; // Attribute not found
        }

    }
}
