﻿namespace ServerlessAuthentication.BasicAuthentication
{
    public interface IBasicAuthenticate
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}
