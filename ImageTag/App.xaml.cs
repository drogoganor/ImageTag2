using ImageTag.Code;
using ImageTag.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text.Json;
using System.Text;
using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace ImageTag
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IConfiguration configuration;
        private ServiceProvider serviceProvider;

        public App()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            configuration = builder.Build();

            ServiceCollection services = new();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddDbContext<ImageTagContext>(options =>
            {
                options.UseSqlite("Data Source = imagetag.db");
            });

            //services.AddSingleton<ImageTagSettings>(LoadSettings());
            services.AddTransient<MainWindow>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
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
