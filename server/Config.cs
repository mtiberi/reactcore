using System;
using System.IO;
using System.Text;

namespace Application
{
    public class Config
    {
        public string AppName = Path.GetFileName(AppDomain.CurrentDomain.FriendlyName);
        public string WwwRoot = Path.Combine(Path.GetDirectoryName(typeof(Config).Assembly.Location), "wwwroot");

        public static void ConsoleLog(string msg)
        {
            System.Console.WriteLine(msg);
        }

        public void Log(string msg)
        {
            ConsoleLog(msg);
        }

        public void Log(Exception e)
        {
            string msg = $"{e.GetType().Name}: {e.Message}";
            ConsoleLog(msg);
        }

        public void Telemetry(string msg)
        {

            ConsoleLog(msg);
        }

        public string GetValue(string name, string defaultValue)
        {
            if (Environment.GetEnvironmentVariables().Contains(name))
                return Environment.GetEnvironmentVariable(name);
            return defaultValue;
        }

        string GetRequiredValie(string key) => GetValue(key, null) ??
            throw new FileNotFoundException("couldn't find required variable in environment: " + key);

        public Config()
        {

            var banner = new StringBuilder()
                .AppendLine("Starting server")
                .AppendLine($"app name: {AppName}")
                .ToString();

            Log(banner);
        }
    }
}
