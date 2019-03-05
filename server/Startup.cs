using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;

namespace Application
{

    class Startup
    {
        void ConfigureClient(IApplicationBuilder appbuilder, string mountPoint, string wwwroot)
        {
            var appPage = Path.Combine(wwwroot, "index.html");

            appbuilder
                // react application static files
                .UseStaticFiles(
                    new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(wwwroot),
                            RequestPath = new PathString(mountPoint),
                    })

                // react application
                .Map(mountPoint, app => app.Run(context =>
                {
                    var path = context.Request.Path.Value;

                    if (string.IsNullOrWhiteSpace(path))
                    {
                        // missing trailing slash: redirect adding it
                        context.Response.Redirect(context.Request.PathBase.Value + "/");
                        return Task.CompletedTask;

                    }

                    if (path == "/")
                    {
                        // application page
                        context.Response.ContentType = "text/html;charset=UTF-8";
                        return context.Response.SendFileAsync(appPage);
                    }

                    // everything else returns 404 not found
                    context.Response.StatusCode = 404;
                    return Task.CompletedTask;

                }));

        }

        public void Configure(IApplicationBuilder appbuilder, Server server, Config config)
        {
            string pathBase = $"/{config.AppName}";
            string urlPrefix = $"{pathBase}/";

            appbuilder
                // fix path
                .Use((context, next) =>
                {
                    var req = context.Request;
                    req.Path = req.PathBase.Value + req.Path.Value;
                    return next();
                })

                // logging
                .Use(async(context, next) =>
                {
                    Exception error = null;
                    try
                    {
                        await next();
                    }
                    catch (Exception e)
                    {
                        error = e;
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync(error.ToString());
                    }

                    var req = context.Features.Get<IHttpRequestFeature>();
                    var msg = $"{DateTime.Now:yyyy-MM-dd hh:mm:ss.ffff} [{context.Response.StatusCode}] {req.Method} {req.RawTarget}";
                    config.Telemetry(msg);
                    if (error != null)
                    {
                        config.Log(error);
                    }

                })

                // redirect home page
                .MapWhen(context =>
                    context.Request.Method == "GET" && context.Request.Path.Value.Length <= 1,
                    app =>
                    {
                        app.Run(context =>
                        {
                            context.Response.Redirect(pathBase);
                            return Task.CompletedTask;
                        });
                    })

                // cors
                .Use((context, next) =>
                {
                    var req = context.Request;

                    // enable cross origin calls
                    var headers = context.Response.Headers;
                    if (req.Headers.ContainsKey("Origin"))
                        headers["Access-Control-Allow-Origin"] = req.Headers["Origin"];
                    else
                        headers["Access-Control-Allow-Origin"] = "*";

                    if (req.Method == "OPTIONS")
                    {
                        headers["Access-Control-Allow-Methods"] = "GET, HEAD, POST, PUT, DELETE, OPTIONS";
                        headers["Access-Control-Allow-Credentials"] = "true";
                        headers["Access-Control-Allow-Headers"] = "Origin, Content-Type, Accept";
                        return Task.CompletedTask;
                    }

                    return next();
                })

                .UseRouter(server.AddRoutes);

            ConfigureClient(appbuilder, pathBase, config.WwwRoot);

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddRouting()
                .AddSingleton<Config>()
                .AddSingleton<Server>();
        }

    }
}
