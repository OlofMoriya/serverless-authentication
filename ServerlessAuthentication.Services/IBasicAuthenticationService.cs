using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Table;
using ServerlessAuthentication.BasicAuthentication;
using System.Threading.Tasks;

namespace ServerlessAuthentication.Services
{
    public interface IBasicAuthenticationService<T> where T: IBasicAuthenticate
    {
        Task<T> AuthenticateAsync(HttpRequest request);
    }
}