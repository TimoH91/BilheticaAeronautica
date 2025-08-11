using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Models;
using BilheticaAeronauticaWeb.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly ITokenService _tokenService;

        public UsersController(UserHelper userHelper, ITokenService tokenService)
        {
            _userHelper = userHelper;
            _tokenService = tokenService;
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid login request.");

            var result = await _userHelper.LoginAsync(model);

            if (!result.Succeeded)
                return Unauthorized("Invalid email or password.");

            var user = await _userHelper.GetUserByEmailAsync(model.Username);

            var token = _tokenService.GenerateToken(user);

            return Ok(new
            {
                AccessToken = token,
                UserId = user.Id,
                UserName = user.UserName
            });
        }
    }
}
