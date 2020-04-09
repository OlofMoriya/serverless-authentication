using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Table;
using ServerlessAuthentication.BasicAuthentication;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace ServerlessAuthentication.Services
{
    public class BasicAuthenticationService : IBasicAuthenticationService<IBasicAuthenticate>
    {
        private readonly IUserService userService;

        public BasicAuthenticationService(IUserService userService) => this.userService = userService;

        public async System.Threading.Tasks.Task<IBasicAuthenticate> AuthenticateAsync(HttpRequest request)
        {
            if (!request.Headers.ContainsKey("Authorization"))
                throw new ArgumentException("Missing Authorization Header");

            if (!(AuthenticationHeaderValue.Parse(request.Headers["Authorization"]) is AuthenticationHeaderValue authHeader))
                throw new ArgumentException("Missing Authorization Header");

            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            var username = credentials[0];
            var password = credentials[1];

            var user = await userService.AuthenticateUserAsync(username, password);

            return user;
        }


    }
}
