namespace Example.Client
{
    using System;
    using System.Windows;

    using Example.Client.Helpers;
    using Example.Client.Settings;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using Rester;

    using Smart.Resolver;
    using Smart.Windows.Resolver;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly IHost host;

        public App()
        {
            host = Host.CreateDefaultBuilder()
                .UseServiceProviderFactory(new SmartServiceProviderFactory())
                .ConfigureServices((context, services) => ConfigureServices(context.Configuration, services))
                .ConfigureContainer<ResolverConfig>(ConfigureContainer)
                .Build();

            RestConfig.Default.UseJsonSerializer(options =>
            {
                options.Converters.Add(new DateTimeConverter());
            });
        }

        private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            var setting = configuration.GetSection("Server").Get<ServerSetting>();
            services.AddHttpClient(ConnectionNames.Server, client =>
            {
                client.BaseAddress = new Uri(setting.BaseUrl);
            });
        }

        private static void ConfigureContainer(HostBuilderContext context, ResolverConfig config)
        {
            config
                .UseAutoBinding()
                .UseArrayBinding()
                .UseAssignableBinding();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await host.StartAsync().ConfigureAwait(false);

            ResolveProvider.Default.UseServiceProvider(host.Services);

            MainWindow = (MainWindow)host.Services.GetRequiredService(typeof(MainWindow));
            MainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await host.StopAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            host.Dispose();
        }
    }
}
