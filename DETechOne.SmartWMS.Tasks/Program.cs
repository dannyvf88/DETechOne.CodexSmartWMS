using DETechOne.SmartWMS.Application.Extensions;
using DETechOne.SmartWMS.Infrastructure.Extensions;
using DETechOne.SmartWMS.Tasks;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSmartWmsApplication();
builder.Services.AddSmartWmsInfrastructure(builder.Configuration);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
