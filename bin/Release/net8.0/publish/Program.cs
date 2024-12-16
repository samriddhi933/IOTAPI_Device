using IOTapi.dbcontext;
using IOTapi.Sevice;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin() // Allow all origins
                          .AllowAnyMethod() // Allow all HTTP methods
                          .AllowAnyHeader()); // Allow all headers
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


GlobalModel.ConnectionString = builder.Configuration.GetConnectionString("MySqlConnection");
builder.Services.AddControllers();
builder.Services.AddScoped<DeviceInstallationService>();



builder.Services.AddDbContext<ferrodbContext>(options =>
    options.UseMySql(
        GlobalModel.ConnectionString,
        ServerVersion.AutoDetect(GlobalModel.ConnectionString) // Auto-detect server version
    )
);


//builder.services.addscoped<componentservice>();

var app = builder.Build();
app.UseCors("AllowAllOrigins");


// Configure the HTTP request pipeline.

//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
