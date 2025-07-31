using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Models;
using BilheticaAeronauticaWeb.Helpers;
using Microsoft.AspNetCore.Authorization;
using BilheticaAeronauticaWeb.Services;
using Vereyon.Web;

namespace BilheticaAeronauticaWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AirplanesController : Controller
    {

        private readonly IAirplaneRepository _airplaneRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IAirplaneService _airplaneService;
        private readonly IBlobHelper _blobHelper;
        private readonly IFlashMessage _flashMessage;

        public AirplanesController(IAirplaneRepository airplaneRepository, IConverterHelper converterHelper,
            IAirplaneService airplaneService, IBlobHelper blobHelper, IFlashMessage flashMessage)
        {
            _airplaneRepository = airplaneRepository;
            _converterHelper = converterHelper;
            _airplaneService = airplaneService;
            _blobHelper = blobHelper;
            _flashMessage = flashMessage;
        }

        // GET: Airplanes
        public async Task<IActionResult> Index()
        {
            return View(_airplaneRepository.GetAll().OrderBy(a => a.Name));
        }

        // GET: Airplanes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AirplaneNotFound");
            }

            var airplane = await _airplaneRepository.GetByIdAsync(id.Value);


            if (airplane == null)
            {
                return new NotFoundViewResult("AirplaneNotFound");
            }

            return View(airplane);
        }

        // GET: Airplanes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Airplanes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AirplaneViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "airplanes");
                }

                var airplane = _converterHelper.ToAirplane(model, imageId, true);

                try
                {
                    await _airplaneRepository.CreateAsync(airplane);

                    _flashMessage.Info("Airplane added successfully!");

                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("This airplane already exists!");
                }
            }
            return View(model);
        }

        // GET: Airplanes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AirplaneNotFound");
            }

            var airplane = await _airplaneRepository.GetByIdAsync(id.Value);

            if (airplane == null)
            {
                return new NotFoundViewResult("AirplaneNotFound");
            }

            var model = _converterHelper.ToAirplaneViewModel(airplane);

            return View(model);
        }

        // POST: Airplanes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AirplaneViewModel model)
        {
            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                try
                {

                    Guid imageId = model.ImageId;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "airplanes");
                    }

                    var editedAirplane =  _converterHelper.ToAirplane(model, imageId, false);

                    var oldAirplane = await _airplaneRepository.GetByIdAsync(model.Id);

                    var canChange = await _airplaneService.AllowAirplaneStatusChange(oldAirplane, editedAirplane);

                    if (canChange)
                    {
                        await _airplaneService.ReassignSeats(oldAirplane, editedAirplane);

                        await _airplaneRepository.UpdateAsync(editedAirplane);
                    }
                    else
                    {
                        _flashMessage.Danger("This airplanes status cannot be edited");
                    }
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!await _airplaneRepository.ExistAsync(model.Id))
                    {
                        return new NotFoundViewResult("AirplaneNotFound");
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

        // GET: Airplanes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AirplaneNotFound");
            }

            var airplane = await _airplaneRepository.GetByIdAsync(id.Value);


            if (airplane == null)
            {
                return new NotFoundViewResult("AirplaneNotFound");
            }

            return View(airplane);
        }

        // POST: Airplanes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var airplane = await _airplaneRepository.GetByIdAsync(id);

            try
            {
                bool canDelete = await _airplaneService.AllowAirplaneDeletion(airplane);

                if (canDelete)
                {
                    await _airplaneRepository.DeleteAsync(airplane);
                    _flashMessage.Info("Airplane deleted!");
                }
                else
                {
                    _flashMessage.Danger("This airplane could not be deleted");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    var errorModel = new ErrorViewModel
                    {
                        ErrorTitle = $"{airplane.Name} provavelmente está a ser usado!!",
                        ErrorMessage = $"{airplane.Name} não pode ser apagado visto haverem encomendas que o usam.<br/><br/>" +
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


        public IActionResult AirplaneNotFound()
        {
            return View();
        }
    }
}
