using IOTapi.dbcontext;
using IOTapi.Model;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace IOTapi.Sevice
{
    public class ComponentService
    {
        private readonly ferrodbContext _context;
        public ComponentService(ferrodbContext context)
        {
            _context = context;
        }
        public List<Component> GetAll()
        {
            try
            {
                return _context.Components.OrderByDescending(c => c.Id).ToList(); // Sorting in descending order
            }
            catch
            {
                return new List<Component>();
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
        SELECT 
            mdt.id, 
            mdt.device_name, 
            c.name, 
            cd.installdate 
        FROM 
            component_device cd
        LEFT JOIN 
            component c ON cd.component_id = c.id
        LEFT JOIN 
            master_device_type mdt ON cd.master_id = mdt.id
        WHERE 
            mdt.id = @deviceId;
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
        public string Addcomponents(List<Component> components, int MasterId)
        {
            try
            {
                foreach (var component in components)
                {
                    // Save the device attribute
                    var Compnents = new Component()
                    {
                        Id = component.Id,
                        Name = component.Name,
                        
                    };
                    _context.Components.Add(component);
                    _context.SaveChanges(); // Save the new attribute

                    // Create and add to the device_details table
                    var componentdevice = new ComponentDevice()
                    {
                        MasterId = MasterId,  // The device ID passed into the method
                        ComponentId = component.Id  // The ID of the newly saved device attribute
                    };
                    _context.ComponentDevices.Add(componentdevice);
                    _context.SaveChanges(); // Save the device detail record
                }

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string Updatecomponent(Component component)
        {
            try
            {
                var Editcomponet = _context.Components.Find(component.Id);

                Editcomponet.Id = component.Id;
                Editcomponet.Name = component.Name;

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
                var deletecomponent = _context.Components.Find(Id);
                _context.Components.Remove(deletecomponent);
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

