using DETechOne.SmartWMS.API.Extensions;
using DETechOne.SmartWMS.API.Security;
using DETechOne.SmartWMS.Application.Dashboard;
using DETechOne.SmartWMS.Application.Extensions;
using DETechOne.SmartWMS.Application.Security;
using DETechOne.SmartWMS.API.Dashboard;
using DETechOne.SmartWMS.Device.Extensions;
using DETechOne.SmartWMS.Infrastructure.Extensions;
using DETechOne.SmartWMS.Installer.Extensions;
using DETechOne.SmartWMS.SAP.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, HttpUserContextService>();
builder.Services.AddSmartWmsJwtAuthentication(builder.Configuration);

builder.Services.AddSmartWmsApplication();
builder.Services.AddSmartWmsDevice();
builder.Services.AddScoped<IDeviceMetricsReader, DeviceMetricsReader>();
builder.Services.AddSmartWmsInfrastructure(builder.Configuration);
builder.Services.AddSmartWmsInstaller(builder.Configuration);
builder.Services.AddSmartWmsSap(builder.Configuration);

var app = builder.Build();

app.UseSmartWmsExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program
{
}
