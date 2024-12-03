namespace AuthService
{
    public class TokenSettings
    {
        public string SecretKey { get; set; }
        public int TokenLifetimeMinutes { get; set; }
    }

}
