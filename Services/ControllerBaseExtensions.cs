using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

public static class ControllerBaseExtensions
{
    public static async Task<string> RenderView<TModel>(this ControllerBase controllerBase, string pathView, TModel model, IRazorViewEngine razorViewEngine, IServiceProvider serviceProvider)
    {
        var viewEngineResult = razorViewEngine.GetView(null, pathView, false);

        if (!viewEngineResult.Success)
            throw new InvalidOperationException($"No se encontró la vista: '{pathView}'.");

        var view = viewEngineResult.View;
        using (var sw = new StringWriter())
        {
            var viewContext = new ViewContext(
                new ActionContext(controllerBase.HttpContext, controllerBase.RouteData, controllerBase.ControllerContext.ActionDescriptor),
                view,
                new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                },
                new TempDataDictionary(controllerBase.HttpContext, serviceProvider.GetRequiredService<ITempDataProvider>()),
                sw,
                new HtmlHelperOptions()
            );

            await view.RenderAsync(viewContext);
            var viewString = sw.ToString();
            return viewString;
        }
    }

    public static string GenerateUrl(this ControllerBase controllerBase, string actionName, string controllerName, IWebHostEnvironment environment)
    {
        string scheme = controllerBase.Request.Scheme;
        string dominio = environment.IsDevelopment()? GetLocalIpAddress(): controllerBase.Request.Host.Host;
        var port = controllerBase.Request.Host.Port;
        var relativeUrl = controllerBase.Url.Action(actionName, controllerName);

        string urlCompleta = $"{scheme}://{dominio}:{port}{relativeUrl}";
        return urlCompleta;
    }

    private static string GetLocalIpAddress()
    {
        string localIp = "";

        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIp = ip.ToString();
                break;
            }
        }

        return localIp;
    }

}