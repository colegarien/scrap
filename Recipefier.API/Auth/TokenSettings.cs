namespace Recipefier.API.Auth
{
    public class TokenSettings
    {
        public string SecretKey { get; set; }
        public int ExpirationInSeconds { get; set; }
    }
}
