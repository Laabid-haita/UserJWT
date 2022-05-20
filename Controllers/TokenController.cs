using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;
using UserWebAPI.Models;
using System.Linq;

namespace UserWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public IConfiguration configuration;
        private readonly DBFormula1Context _formulaContext;
        public TokenController(IConfiguration config, DBFormula1Context context)
        {
            _formulaContext = context;
            configuration = config;    
        }
        [HttpPost]

        public async Task<IActionResult> Post(User userData)
        {
            if (userData != null && userData.Email!=null && userData.Password != null)
            {

                //var user = await GetUser(userData.Email, userData.Password);
                var user = _formulaContext.Users.SingleOrDefault(x => x.Email == userData.Email);
                if (user != null || BC.Verify(userData.Password, user.Password))
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub,configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
                        new Claim("id", user.IdU.ToString()),
                        new Claim("Email", user.Email),
                        new Claim("Password", user.Password),
                        new Claim("Pseudo", user.Pseudo),
                        new Claim("Role", user.Role),
                    };
                    var ket = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(ket, SecurityAlgorithms.HmacSha256);
    
                    var token = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddHours(2), signingCredentials: signIn);
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                
                
                }
                else
                {
                    return BadRequest("invalid email or password");
                }

            }
            else
            {
                return BadRequest();
            }
            
        }
        private async Task<User> GetUser(string email, string password)
        {
            return await _formulaContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

    }
}
