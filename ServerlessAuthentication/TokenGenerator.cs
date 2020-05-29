using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.WindowsAzure.Storage.Table;
using ServerlessAuthentication.BasicAuthentication;
using ServerlessAuthentication.Model.Model;
using ServerlessAuthentication.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessAuthentication
{
    public class TokenGenerator
    {
        private const int daysValid = 1;
        private const string issuer = "Issuer";
        private const string authority = "Authority";
        private const string symetricSecurity = "Veeeery sneakyyy";

        private readonly IBasicAuthenticationService<IBasicAuthenticate> basicAuthenticationService;

        public TokenGenerator(IBasicAuthenticationService<IBasicAuthenticate> basicAuthenticationService) => this.basicAuthenticationService = basicAuthenticationService;

        [FunctionName("GenerateToken")]
        public async Task<IActionResult> Run (
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GenerateToken")] HttpRequest request
        )
        {
            var user = await basicAuthenticationService.AuthenticateAsync(request);
            if (user is null) return new UnauthorizedResult();

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            if (user is User userModel)
            {
                claims.AddClaim(new Claim("name", $"{userModel.FirstName} {userModel.LastName}"));
                claims.AddClaim(new Claim("systems", userModel.Subject));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(issuer: issuer,
                audience: authority,
                subject: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(daysValid),
                signingCredentials:
                new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.Default.GetBytes(symetricSecurity)),
                        SecurityAlgorithms.HmacSha256Signature));

            return new OkObjectResult(tokenHandler.WriteToken(token));
        }
    }
}
