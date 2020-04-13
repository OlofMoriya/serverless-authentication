namespace ServerlessAuthentication.Model.InputModel
{
    public class UserData : IValidateData
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Subject { get; set; }
        public bool ValidateData() => true;
    }
}
