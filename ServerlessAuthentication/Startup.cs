using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ServerlessAuthentication.BasicAuthentication;
using ServerlessAuthentication.Model.Model;
using ServerlessAuthentication.Services;
using ServerlessAuthentication.SettingsModel;
using System.IO;

[assembly: FunctionsStartup(typeof(ServerlessAuthentication.Startup))]

namespace ServerlessAuthentication
{
    class Startup : FunctionsStartup
    {
        public IConfiguration? Configuration { get; }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .Build();

            if (!(config.GetSection("storageSettings").GetValue(typeof(string), "connectionString") is string connectionString)) {
                throw new InvalidDataException("ConnectionString is not specified in the settings");
            }

            builder.Services.AddScoped(provider =>
            {
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                return tableClient;
            });

            builder
                .Services
                .AddOptions<TokenSettings>()
                .Configure<IConfiguration>((settings, configuration) => { configuration.Bind(settings); });

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped(typeof(IBasicAuthenticationService<IBasicAuthenticate>), typeof(BasicAuthenticationService));
        }
    }
}
