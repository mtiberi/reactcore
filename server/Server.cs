using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace Application
{
    public class Server
    {
    
        Config Config;

        public Server(Config config)
        {
            Config = config;
        }

        string UrlParam(HttpContext ctx, string name) =>
            ctx.GetRouteData().Values[name].ToString();

        RequestDelegate Route(RequestDelegate action) => action;
        
        RequestDelegate Route(Func<HttpContext, string, Task> action, string name) =>
            ctx => action(ctx, UrlParam(ctx, name));

        RequestDelegate Route(Func<HttpContext, string, string, Task> action, string name1, string name2) =>
            ctx => action(ctx, UrlParam(ctx, name1), UrlParam(ctx, name2));

        internal void AddRoutes(IRouteBuilder builder)
        {
            var root = $"/{Config.AppName}";
            
            (string, string, RequestDelegate) [] routes = {
                /*
                ("POST", "/upload/{fileId}", Route(PostAttribute, "fileid")),
                */
            };

            foreach (var(verb, path, handler) in routes)
                builder.MapVerb(verb, root+path, handler);
        }

     

        async Task<byte[]> GetBody(HttpContext context)
        {
            using(var s = new MemoryStream())
            {
                await (context.Request.Body.CopyToAsync(s));
                return s.ToArray();
            }
        }

    }

}
