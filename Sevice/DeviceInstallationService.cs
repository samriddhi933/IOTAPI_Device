using IOTapi.dbcontext;
using IOTapi.Model;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace IOTapi.Sevice
{
    public class DeviceInstallationService
    {       
        
            private readonly ferrodbContext _context;
            private readonly ILogger<DeviceInstallationService> _logger;

            public DeviceInstallationService(ferrodbContext context, ILogger<DeviceInstallationService> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<(bool, string)> SaveDeviceInstallationAsync(Deviceinstallation installation, List<IFormFile> images)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Save the DeviceInstallation
                    _context.Deviceinstallations.Add(installation);
                    await _context.SaveChangesAsync(); // Persist the installation to generate ID

                    // Handle image uploads
                    foreach (var image in images)
                    {
                        if (!ValidateImageFormat(image))
                        {
                            _logger.LogWarning("Invalid image format detected during save.");
                            return (false, "Invalid image format");
                        }

                        // Save image to the server and store the path in the database
                        var imageUrl = await SaveImageAsync(image);

                    // Save image details linked to the installation
                    var imageRecord = new Imagedetail
                    {
                        UniqueId = installation.Id.ToString(),  // Link image to DeviceInstallation
                        Imageurl = imageUrl,
                        ImageTypeId = 2,
                    };

                        _context.Imagedetails.Add(imageRecord);
                    }

                    // Commit transaction
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return (true, "Installation and images saved successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occurred during save: {ex}");
                    await transaction.RollbackAsync();
                    return (false, $"An error occurred: {ex.Message}");
                }
            }

            // Make ValidateImageFormat public so that it can be accessed from the controller
            public bool ValidateImageFormat(IFormFile image)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(image.FileName).ToLower();
                var maxFileSize = 5 * 1024 * 1024; // 5 MB limit

                return allowedExtensions.Contains(fileExtension) && image.Length <= maxFileSize;
            }

            private async Task<string> SaveImageAsync(IFormFile image)
            {
                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "device_images");
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                // Generate a unique file name
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                var filePath = Path.Combine(uploadDirectory, uniqueFileName);

                // Save the image
                await using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream);

                // Return URL relative to wwwroot
                return $"/uploads/device_images/{uniqueFileName}";
            }

        public IEnumerable<dynamic> GetDeviceAggignById(int roleId, int entityId)
        {
            var query = @"
        SELECT 
            d.id, 
            d.imie, 
            mdt.device_name, 
            a.addressline1, 
            CONCAT(p.firstname, ' ', p.lastname) AS contact_person, 
            di.installDate,
            di.Remarks, 
            ee.Name, 
            imd.Imageurl 
        FROM 
            deviceinstallation di
        LEFT JOIN 
            (SELECT * FROM imagedetails WHERE ImageTypeId = 2) imd ON imd.UniqueId = di.id
        LEFT JOIN 
            device d ON d.id = di.DeviceId
        LEFT JOIN 
            address a ON a.id = di.AddressId
        LEFT JOIN 
            person p ON p.id = di.approverPersonId
        LEFT JOIN 
            (SELECT e.id, r.id AS entity, r.name 
             FROM entityrelationship er
             LEFT JOIN entity e ON e.id = er.entityid
             LEFT JOIN entity r ON r.id = er.RelationshipId) ee ON ee.entity = di.EntityId
        LEFT JOIN 
            master_device_type mdt ON mdt.id = d.devicetypeid
        WHERE 
            ee.id = @roleId AND ee.entity = @entityId;
    ";

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;

                var roleIdPara = command.CreateParameter();
                roleIdPara.ParameterName = "@roleId";
                roleIdPara.Value = roleId;
                command.Parameters.Add(roleIdPara);

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
                    throw new Exception($"Error executing query for roleId {roleId} and entityId {entityId}: {ex.Message}", ex);
                }
                finally
                {
                    _context.Database.CloseConnection();
                }
            }
        }

    }

}
