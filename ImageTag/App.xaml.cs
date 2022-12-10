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
using ImageTag.ViewModel;

namespace ImageTag
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // https://blogs.msmvps.com/bsonnino/2021/02/13/implementing-the-mvvm-pattern-in-a-wpf-app-with-the-mvvm-community-toolkit/
        public new static App Current => (App)Application.Current;
        public static IHost AppHost { get; private set; }

        public ImageTagViewModel ViewModel => AppHost.Services.GetService<ImageTagViewModel>();

        private IConfiguration configuration;
        private ServiceProvider serviceProvider;

        public App()
        {
            // https://stackoverflow.com/questions/73512443/dependency-injection-in-wpf-on-net-core-6
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
            services.AddSingleton<ViewModel.ImageTagViewModel>();
            services.AddTransient<MainWindow>();
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            //var mainWindow = serviceProvider.GetService<MainWindow>();
            //mainWindow.Show();


            await AppHost!.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            startupForm.Show();
        }

        //protected override async void OnExit(ExitEventArgs e)
        //{
        //    using (AppHost)
        //    {
        //        await AppHost.StopAsync(TimeSpan.FromSeconds(5));
        //    }

        //    base.OnExit(e);
        //}

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
