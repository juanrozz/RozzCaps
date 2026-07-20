using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RozzCaps.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEYNAME = "X-Admin-Key";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var configuracion = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKeyValida = configuracion.GetValue<string>("AdminSettings:SecretKey");

            if (!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Acceso denegado. No se proporciono la clave de administracion."
                };

                return;
            }

           if (!apiKeyValida!.Equals(extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Acceso denegado. La clave de administración es inválida."
                };
                return;
            }

            await next();
        }
    }
}
