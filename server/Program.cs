using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;

namespace Application
{
    using Console = System.Console;

    public class Program
    {
        public static string ReadVariable(string key, string defaultValue) => Environment.GetEnvironmentVariable(key) ?? defaultValue;

        public static string GetServerUrl(string defaultHost, string defaultPort)
        {
            string host = ReadVariable("HOST", defaultHost);
            if (host.Any(ch => !char.IsLetterOrDigit(ch) && ch != '.' && ch != '-' && ch != '*'))
                throw new InvalidDataException("invalid server HOST: " + host);

            string port = ReadVariable("PORT", defaultPort);
            if (port.Any(ch => !char.IsDigit(ch)))
                throw new InvalidDataException("invalid server PORT: " + port);

            return "http://" + host + ":" + port;
        }

        public static void LoadEnvironment(string filename)
        {
            if (File.Exists(filename))
            {
                foreach (var line in File.ReadAllLines(filename))
                {
                    var p = line.IndexOf("=");
                    if (p > 0)
                    {
                        var key = line.Substring(0, p).Trim();
                        var value = line.Substring(p + 1).Trim();
                        Environment.SetEnvironmentVariable(key, value);
                    }
                }
            }
        }

        public static void Main(string[] args)
        {
            LoadEnvironment("environment.ini");
            new WebHostBuilder()
                .UseKestrel()
                .UseUrls(GetServerUrl("*", "5000"))
                .UseStartup<Startup>()
                .Build()
                .Run();

        }
    }
}
