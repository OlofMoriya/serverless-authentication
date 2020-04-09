﻿using Microsoft.WindowsAzure.Storage.Table;
using ServerlessAuthentication.Model.InputModel;
using ServerlessAuthentication.Model.Model;
using System.Threading.Tasks;

namespace ServerlessAuthentication.Services
{
    public interface IUserService
    {
        (User, string) CreateUser(UserData userData);
        UserData CreateUserData(User user);
        User SetPassword(User user, string password);
        Task<User> AuthenticateUserAsync(string username, string password);
    }
}