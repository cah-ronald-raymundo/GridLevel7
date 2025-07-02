using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NetAPIGrid.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NetAPIGrid.Service
{

    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public async Task<UserRequest?> Authenticate(UserRequest login)
        {


            login = new UserRequest();
            login.Username = "gridadmin";

            try
            {

                var issuer = _configuration["JwtConfig:Issued"];
                var audience = _configuration["JwtConfig:Audience"];
                var key = _configuration["JwtConfig:Key"];
                var tokenExpiryTimeStamp = DateTime.UtcNow.AddDays(1);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Name, login.Username),

                    }),
                    //Audience = audience,
                    Expires = tokenExpiryTimeStamp,
                    Issuer = issuer,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha384Signature),


                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var accessToken = tokenHandler.WriteToken(securityToken);

                return new UserRequest
                {
                    AccessToken = accessToken,
                    Username = login.Username,
                    ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds
                };

            }
            catch (Exception ex)
            {

                return login;
            }

        }
    }
}
