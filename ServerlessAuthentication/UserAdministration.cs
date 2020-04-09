using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using ServerlessAuthentication.Model.InputModel;
using ServerlessAuthentication.Services;

namespace ServerlessAuthentication
{
    public class UserAdministration
    {
        private readonly IUserService userService;

        public UserAdministration(IUserService userService)
        {
            this.userService = userService;
        }

        [FunctionName("AddUser")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "AddUser")] HttpRequest req,
            [Table("users")] CloudTable userTable,
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

            var (user, password) = userService.CreateUser(userData);

            user.RowKey = $"U{user.Email}";
            user.PartitionKey = $"U{user.Email.Substring(0, 2)}";

            var userInsertOperation = TableOperation.Insert(user);
            var userInserResult = await userTable.ExecuteAsync(userInsertOperation);
            //Handle confict with correct status
            if (userInserResult.HttpStatusCode >= 300)
            {
                return new EmptyResult();
            }

            return new CreatedResult("Storage", new { User = userService.CreateUserData(user), Password = password });
        }
    }
}
