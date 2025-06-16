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

namespace BilheticaAeronauticaWeb.Controllers
{
    public class AirplanesController : Controller
    {

        private readonly IAirplaneRepository _airplaneRepository;
        private readonly IConverterHelper _converterHelper;

        public AirplanesController(IAirplaneRepository airplaneRepository, IConverterHelper converterHelper)
        {
            _airplaneRepository = airplaneRepository;
            _converterHelper = converterHelper;
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

                
                //TODO this will be empty as the blob helper creates the guid, when I change it
                Guid imageId = Guid.NewGuid();


                //TODO when blob container is set up, remove file stream and use the blobhelper
                //if (model.ImageFile != null && model.ImageFile.Length > 0)
                //{
                //    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");

                //}

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "airplanes", $"{imageId}.jpg");

                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

                    using (var stream = new FileStream(destinationPath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }
                }

                var airplane = _converterHelper.ToAirplane(model, imageId, true);


                try
                {
                    await _airplaneRepository.CreateAsync(airplane);

                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    //TODO Flashmessage"This airplane already exists!"
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
            if (ModelState.IsValid)
            {
                try
                {
                    //TODO: Change when blobstorage is setup
                    //Guid imageId = model.ImageId;
                    Guid imageId = Guid.NewGuid();


                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {

                            var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "airplanes", $"{imageId}.jpg");

                            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

                            using (var stream = new FileStream(destinationPath, FileMode.Create))
                            {
                                await model.ImageFile.CopyToAsync(stream);
                            }                     
                        //TODO uncomment code below when blob helper is setup
                        //imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
                    }

                    var airplane =  _converterHelper.ToAirplane(model, imageId, false);

                    await _airplaneRepository.UpdateAsync(airplane);
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
                await _airplaneRepository.DeleteAsync(airplane);
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

        //private bool AirplaneExists(int id)
        //{
        //    return _airplaneRepository.Any(e => e.Id == id);
        //}

        public IActionResult AirplaneNotFound()
        {
            return View();
        }
    }
}
