using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Recipefier.API.Auth
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
    }

    class UserService : IUserService
    {
        private readonly TokenSettings _tokenSettings;

        public UserService(IOptions<TokenSettings> tokenSettings)
        {
            _tokenSettings = tokenSettings.Value;
        }

        private List<User> _users = new List<User> // TODO some kinda DB
        {
            new User { Id = 1, Username = "admin", Password = "admin" }
        };

        public User Authenticate(string username, string password)
        {
            var aUser = _users.FirstOrDefault(u => u.Username == username && u.Password == password); // TODO would actually want to be storing these passwords encrypted and salted...

            if (aUser == null)
            {
                return null;
            }

            aUser.Token = GenerateToken(aUser);
            aUser.Password = ""; // for security

            return aUser;
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_tokenSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddSeconds(_tokenSettings.ExpirationInSeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
