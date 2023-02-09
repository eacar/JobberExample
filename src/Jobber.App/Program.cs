using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rpa.Enums;
using Rpa.Extensions;
using Rpa.Log.Loggers;
using Rpa.WinService.Extensions;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Jobber.App
{
    public class Program
    {
        #region Fields

        private static readonly string Namespace = typeof(Program).Namespace;
        private static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        #endregion

        #region Methods - Public

        public static async Task Main(string[] args)
        {
            SharpLogger.Start();
             
            var configuration = GetConfiguration();
            var isDebug = Debugger.IsAttached || !configuration["IsRunAsService"].Convert<bool>();

            try
            {
                SharpLogger.LogInfo($"{AppName} is starting...");

                var host = new HostBuilder()
                    .UseWindowsService()
                    .ConfigureAppConfiguration(builder =>
                    {
                        builder.Sources.Clear();
                        builder.AddConfiguration(configuration);
                    })
                    .UseStartup<Startup>()
                    .ConfigureServices((hostContext, services) =>
                    {
                        //Do good thing here if you want
                    });

                if (!isDebug)
                {
                    await host.RunServiceAsync<Service>(); //This <Service> class is an override. You don't have to use it unless some specific thing you wanna do onStop and onStart
                }
                else
                {
                    await host.RunConsoleAsync();
                }
            }
            catch (Exception ex)
            {
                SharpLogger.LogError(ex, "Something went wrong");
                if (isDebug)
                {
                    Console.WriteLine(ex);
                    Console.ReadKey();
                }
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        #endregion

        #region Methods - Private
        
        private static IConfiguration GetConfiguration()
        {
            var environmentType = EnvironmentType.Dev;

            foreach (EnvironmentType type in Enum.GetValues<EnvironmentType>())
            {
                var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings.{type.ToString().ToLower()}.json");

                if (File.Exists(file))
                {
                    SharpLogger.LogInfo($"Found settings '{file}'");
                    SharpLogger.LogInfo($"EnvironmentType is {type}");
                    break;
                }
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"appsettings.{environmentType.ToString().ToLower()}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        #endregion
    }
}