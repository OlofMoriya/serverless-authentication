using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Table;
using ServerlessAuthentication.BasicAuthentication;
using ServerlessAuthentication.Model.InputModel;
using ServerlessAuthentication.Model.Model;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
//Nuget Microsoft.Azure.WebJobs.Extensions.Storage

namespace ServerlessAuthentication.Services
{
    public class UserService : IUserService
    {
        private static readonly RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
        private readonly CloudTable cloudTable;

        public UserService(CloudTableClient cloudTableClient)
        {
            this.cloudTable = cloudTableClient.GetTableReference("users");
        }

        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            var retrieveOperation = TableOperation.Retrieve<User>($"U{email.Substring(0, 2)}", $"U{email}");
            var tableResult = await cloudTable.ExecuteAsync(retrieveOperation);
            if (tableResult.Result is User user && user.Password == HashPassword(password, user.Salt)) {
                user.Password = String.Empty;
                return user;
            }
            return null;
        }

        public (User, string) CreateUser(UserData userData)
        {
            _ = userData.ValidateData() ? true : throw new ArgumentException("UserData failed on validation.");

            var user = new User
            {
                LastName = userData.LastName,
                FirstName = userData.FirstName,
                Email = userData.Email,
                Subject = userData.Subject
            };

            byte[] saltBytes = new byte[6];
            random.GetBytes(saltBytes);
            var salt = Encoding.UTF8.GetString(saltBytes, 0, saltBytes.Length);
            user.Salt = salt;

            byte[] passwordBytes = new byte[10];
            random.GetBytes(passwordBytes);
            var password = Encoding.UTF8.GetString(passwordBytes, 0, passwordBytes.Length);
            var hashedPassword = HashPassword(password, salt);
            user.Password = hashedPassword;

            return (user, password);
        }

        public UserData CreateUserData(User user)
        {
            return new UserData
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Subject = user.Subject
            };
        }

        public User SetPassword(User user, string password)
        {
            user.Password = HashPassword(password, user.Salt);
            return user;
        }

        private string HashPassword(string password, object salt)
        {
            var saltedPassword = $"{password}{salt}"; // Should have another configuration salt from config file
            var hashedPassword = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Encoding.UTF8.GetString(hashedPassword);
        }
    }
}
