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
        private readonly IConverterHelper _converterHelper;

        public UsersController(IUserRepository userRepository, UserManager<User> userManager, IConverterHelper converterHelper)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _converterHelper = converterHelper;
        }

        public async Task<IActionResult> Index()
        {
            //var users = _userRepository.GetAll().OrderBy(u => u.UserName).ToList();

            var users = await _userRepository.GetAllWithRoles();

           // var userViewModels = new List<UserViewModel>();

           // foreach (var user in users)
           // {
           //     var roles = await _userManager.GetRolesAsync(user);
           //     userViewModels.Add(new UserViewModel
           //     {
           //         //Id = user.Id,
           //         UserName = user.UserName,
           //         Email = user.Email,
           //         Role = roles.FirstOrDefault() ?? "No role"
           //     });
           //}

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
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _converterHelper.ToUser(model, false);

                    await _userRepository.UpdateAsync(user);
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
    }
}
