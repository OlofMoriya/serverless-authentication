using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessAuthentication.Model.InputModel;
using ServerlessAuthentication.Services;
using System.IO;
using System.Threading.Tasks;

namespace ServerlessAuthentication
{
    public class UserAdministration
    {
        private readonly IUserService userService;

        public UserAdministration(IUserService userService) => this.userService = userService;

        [FunctionName("AddUser")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "AddUser")] HttpRequest req,
            ILogger log
        )
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();
            var userData = JsonConvert.DeserializeObject<UserData>(content);

            if (!userData.ValidateData())
            {
                log.LogDebug("Data could not be validated!");
                return new BadRequestResult();
            }

            var (user, password) = await userService.CreateUserAsync(userData);

            if (user is null)
            {
                return new EmptyResult();
            }

            return new CreatedResult("Storage", new { User = userService.CreateUserData(user), Password = password });
        }
    }
}
