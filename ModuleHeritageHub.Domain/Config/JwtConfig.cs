namespace ModuleHeritageHub.Domain.Config
{
    public class JwtConfig
        {
            public string SecretKey { get; set; } = string.Empty;
            public string Issuer { get; set; } = string.Empty;
            public string Audience { get; set; } = string.Empty;
            public string TokenLifetime { get; set; } = string.Empty;
        }

}