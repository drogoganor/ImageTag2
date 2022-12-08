using ImageTag.Code;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text.Json;
using System.Text;
using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace ImageTag
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IConfiguration configuration;
        private ServiceProvider serviceProvider;

        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host
                .CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    ConfigureServices(services);

                    //services.AddSingleton<MainWindow>();

                    IConfiguration configuration;

                    configuration = new ConfigurationBuilder()
                        .AddJsonFile(@"appsettings.json")
                        .Build();
                })
                .Build();


            //var builder = new ConfigurationBuilder()
            // .SetBasePath(Directory.GetCurrentDirectory())
            // .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            //configuration = builder.Build();

            //ServiceCollection services = new();

            ////services.ConfigureOptions(configuration);

            //ConfigureServices(services);
            //serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ImagetagContext>(options =>
            {
                options.UseSqlite("Data Source = imagetag.db");
            });

            services.AddSingleton<ImageTagSettings>(LoadSettings());
            services.AddSingleton<ImageTag.Code.ImageTag>();
            services.AddTransient<MainWindow>();
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            //var mainWindow = serviceProvider.GetService<MainWindow>();
            //mainWindow.Show();


            await AppHost!.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            startupForm.Show();

            base.OnStartup(e);
        }

        private ImageTagSettings LoadSettings()
        {
            var settingsPath = Path.Combine(Environment.CurrentDirectory, @"settings.json");

            if (File.Exists(settingsPath))
            {
                using var fs = File.OpenRead(settingsPath);
                using var sr = new StreamReader(fs, Encoding.UTF8);
                string content = sr.ReadToEnd();

                return JsonSerializer.Deserialize<ImageTagSettings>(content);
            }
            else
            {
                return new ImageTagSettings();
            }
        }
    }
}
