using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ServerlessAuthentication.BasicAuthentication;
using ServerlessAuthentication.Model.Model;
using ServerlessAuthentication.Services;

[assembly: FunctionsStartup(typeof(ServerlessAuthentication.Startup))]

namespace ServerlessAuthentication
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped(provider =>
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                return tableClient;
            });
            
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped(typeof(IBasicAuthenticationService<IBasicAuthenticate>), typeof(BasicAuthenticationService));
        }
    }
}
