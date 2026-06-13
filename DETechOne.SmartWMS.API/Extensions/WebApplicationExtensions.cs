using DETechOne.SmartWMS.API.Middleware;

namespace DETechOne.SmartWMS.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseSmartWmsExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
}
