using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Models;
using BilheticaAeronauticaWeb.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly ITokenService _tokenService;
        private readonly IMailHelper _mailHelper;
        private readonly IBlobHelper _blobHelper;

        public UsersController(IUserHelper userHelper, ITokenService tokenService,
            IMailHelper mailHelper, IBlobHelper blobHelper)
        {
            _userHelper = userHelper;
            _tokenService = tokenService;
            _mailHelper = mailHelper;
            _blobHelper = blobHelper;
        }

        [HttpPost("Login")]
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
                TokenType = "bearer",
                UserId = user.Id,
                UserName = user.UserName
            });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterNewUserViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid login request.");

            var user = await _userHelper.GetUserByEmailAsync(model.Username);

            if (user == null)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");

                }

                user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Username,
                    UserName = model.Username,
                    ImageId = imageId,
                    Role = "Customer"

                };

                var result = await _userHelper.AddUserAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "The user couldn't be created." });
                }
                else
                {
                    await _userHelper.AddUserToRoleAsync(user, "Customer");
                }

                string tokenLink = await _tokenService.GenerateEmailConfirmationLinkAsync(user);


                Response response = _mailHelper.SendEmail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                                                                         $"To allow the user, " +
                                                                        $"please click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");


                if (!response.IsSuccess)
                {
                    return StatusCode(500, new { message = "Error sending email." });
                }

            }

            return Ok(new 
            { 
                Message = "The instructions to login as a user has been sent to email.",
                UserId = user.Id,
            });

        }
        

        [HttpPost("RecoverPassword")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userHelper.GetUserByEmailAsync(model.Email);

            if (user == null)
                return NotFound(new { message = "The email doesn't correspond to a registered user." });

            var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

            var link = Url.Action(
                "ResetPassword",
                "Account",
                new { token = myToken },
                protocol: HttpContext.Request.Scheme);

            var response = _mailHelper.SendEmail(model.Email, "Shop Password Reset",
                $"<h1>Shop Password Reset</h1>" +
                $"To reset the password click in this link:</br></br>" +
                $"<a href=\"{link}\">Reset Password</a>");

            if (!response.IsSuccess)
                return StatusCode(500, new { message = "Error sending email." });

            return Ok(new { message = "The instructions to recover your password have been sent to email." });
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Email != null)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);

                if (user == null)
                    return NotFound(new { message = "The email doesn't correspond to a registered user." });

                var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { message = "Password change failed.", errors });
                }

                return Ok(new { message = "Password changed successfully." });
            }

            return BadRequest(new { message = "Email is required." });
        }

    }
}
