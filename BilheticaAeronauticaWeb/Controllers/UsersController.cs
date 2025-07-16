using Microsoft.AspNetCore.Mvc;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Models;
using System.Diagnostics;
using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BilheticaAeronauticaWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IMailHelper _mailHelper;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(IUserRepository userRepository,UserManager<User> userManager,
            IConverterHelper converterHelper,IUserHelper userHelper,IMailHelper mailHelper, RoleManager<IdentityRole> roleManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllWithRoles();

            return View(users);
        }

        // GET: Flights/Details/5

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            var user = await _userRepository.GetByIdAsync(id);


            if (user == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }

        //POST: Flights/Create
        //To protect from overposting attacks, enable the specific properties you want to bind to.
        //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _converterHelper.ToUser(model, true);

                try
                {
                    await _userRepository.CreateAsync(user, model.Role, model.Password);

                    await MailNewUser(user);

                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            return View(model);
        }


        //GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            var model = await _converterHelper.ToUserViewModelAsync(user);

            return View(model);

        }

        //POST: Flights/Edit/5
        //To protect from overposting attacks, enable the specific properties you want to bind to.
        //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            //if (!string.IsNullOrEmpty(model.Id))
            //{
            //    var user = await _userRepository.GetByIdAsync(model.Id);
            //    model.Password = user.PasswordHash;
            //}

            ModelState.Remove("Password");

            if (ModelState.IsValid)
             {
                try
                {
                    var oldUser = await _userRepository.GetByIdAsync(model.Id);

                    await UpdateRole(model, oldUser);

                    var user = await _converterHelper.ToUser(model, false);

                    await _userHelper.UpdateUserAsync(user);
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!await _userRepository.ExistAsync(model.Id))
                    {
                        return new NotFoundViewResult("UserNotFound"); ;
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        //GET: Users/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            var flight = await _userRepository.GetByIdAsync(id);

            if (flight == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            return View(flight);
        }

        //POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            try
            {
                await _userRepository.DeleteAsync(user);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    var errorModel = new ErrorViewModel
                    {
                        ErrorTitle = $"{user.Email} provavelmente está a ser usado!!",
                        ErrorMessage = $"{user.Email} não pode ser apagado visto haverem encomendas que o usam.<br/><br/>" +
                                      $"Experimente primeiro apagar todas as encomendas que o estão a usar," +
                                      $"e torne novamente a apagá-lo"
                    };

                    return View("Error", errorModel);
                }


                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = "Erro de base de dados",
                    ErrorMessage = "Ocorreu um erro inesperado ao tentar apagar o aeroporto."
                });

            }
        }

        private async Task MailNewUser(User newUser)
        {

            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(newUser);
            string tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = newUser.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme);

            Response response = _mailHelper.SendEmail(newUser.Email, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                                                                     $"To allow the user, " +
                                                                    $"please click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");


            if (response.IsSuccess)
            {

                ViewBag.Message = "The instructions to allow you user has been sent to email";
                //return View(model);
            }


            ModelState.AddModelError(string.Empty, "The user couldn't be logged.");
        }


        private async Task UpdateRole(UserViewModel model, User oldUser)
        {
            if (model.Role != oldUser.Role)
            {
                try
                {
                    await _userManager.RemoveFromRoleAsync(oldUser, oldUser.Role);
                    await _userHelper.AddUserToRoleAsync(oldUser, model.Role);
                }
                catch (DbUpdateException ex)
                {

                }

            }
        }
    }
}
