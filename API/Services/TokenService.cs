using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {

            //adding our claims 
            var claims = new List<Claim> 
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };
            
            // creating some credentials 
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature); 

            // describing how our token is going to look 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };
            //just something we need 
            var tokenHandler = new JwtSecurityTokenHandler();
            //we create the token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //we return the token
            return tokenHandler.WriteToken(token);
        }
    }
}