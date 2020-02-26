using System;

namespace Example.Services.AuthService
{
    public class AuthToken
    {
       public string AccessToken { get; set; }
       public string RefreshToken { get; set; }
       public DateTimeOffset ExpiresAt { get; set; }
    }
}