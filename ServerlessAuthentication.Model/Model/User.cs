using Microsoft.WindowsAzure.Storage.Table;
using ServerlessAuthentication.BasicAuthentication;
using System.Collections.Generic;

namespace ServerlessAuthentication.Model.Model
{
    public class User : TableEntity, IBasicAuthenticate
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Subject { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
    }
}