using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace BilheticaAeronauticaWeb.Services
{

    public class TokenService : ITokenService
    {

        private readonly IConfiguration _configuration;
        private readonly IUserHelper _userHelper;

        public TokenService(IConfiguration configuration, IUserHelper userHelper)
        {
            _configuration = configuration;
            _userHelper = userHelper;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Tokens:Issuer"],
                _configuration["Tokens:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateEmailConfirmationLinkAsync(User user)
        {
            string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            string encodedToken = WebUtility.UrlEncode(token); // always encode tokens for URLs
            return $"{_configuration["Tokens:Issuer"]}account/confirmemail?userid={user.Id}&token={encodedToken}";
        }
    }
}
