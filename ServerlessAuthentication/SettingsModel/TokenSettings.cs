namespace ServerlessAuthentication.SettingsModel
{
    class TokenSettings
    {
        public string Issuer { get; set; }
        public string Authority { get; set; }
        public string Secret { get; set; }
    }
}
