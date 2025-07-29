using System.Diagnostics;
using System.Runtime.InteropServices;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Models;
using BilheticaAeronauticaWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Syncfusion.EJ2;

namespace BilheticaAeronauticaWeb.Controllers
{
    [Authorize]
    public class AirportsController : Controller
    {
        private readonly IAirportRepository _airportRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IAirportService _airportService;
        //private readonly IBlobHelper _blobHelper;

        public AirportsController(IAirportRepository aeroportoRepository,
            IConverterHelper converterHelper,
            ICountryRepository countryRepository,
            IAirportService airportService)
        {
            _airportRepository = aeroportoRepository;
            _converterHelper = converterHelper;
            _countryRepository = countryRepository;
            _airportService = airportService;
            //_blobHelper = blobHelper;
        }

        // GET: Airports
        public async Task<IActionResult> Index()
        {
            ViewBag.countryData = await _airportRepository.GetAirportCountriesForMapAsync();

            ViewBag.worldmap = GetWorldMap();

            var airports = _airportRepository.GetAll().OrderBy(a => a.Name).ToList();

            return View(airports);
        }

        // GET: Airports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AirportNotFound");
            }

            var airport = await _airportRepository.GetByIdAsync(id.Value);


            if (airport == null)
            {
                return new NotFoundViewResult("AirportNotFound");
            }

            return View(airport);
        }

        // GET: Airports/Create
        public IActionResult Create()
        {
            ViewBag.Countries = _countryRepository.GetComboCountries(true);

            ViewBag.Cities = new List<SelectListItem>
            {
                new SelectListItem { Text = "(Select a country first...)", Value = "0" }
            };

            return View();
        }

        // POST: Airports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AirportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var airport = await _converterHelper.ToAirport(model, true);

                try
                {
                    await _airportRepository.CreateAsync(airport);

                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    //TODO Flashmessage"Este aeroporto já existe!"
                }
            }
            return View(model);
        }

        // GET: Airports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AirportNotFound");
            }

            var airport = await _airportRepository.GetByIdAsync(id.Value);

            if (airport == null)
            {
                return new NotFoundViewResult("AirportNotFound");
            }

            var model = _converterHelper.ToAirportViewModel(airport);

            ViewBag.Countries = _countryRepository.GetComboCountries(false);

            ViewBag.Cities = new List<SelectListItem>
            {
                new SelectListItem { Text = "(Select a country first...)", Value = "0" }
            };

            return View(model);

        }

        // POST: Airports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AirportViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var airport = await _converterHelper.ToAirport(model, false);

                    var canEdit = await _airportService.AllowAirportDeletionOrUpdate(airport);

                    if (canEdit)
                    {
                        await _airportRepository.UpdateAsync(airport);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _airportRepository.ExistAsync(model.Id))
                    {
                        return NotFound();
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

        [HttpPost]
        [Route("Airports/GetCitiesAsync")]
        public async Task<JsonResult> GetCitiesAsync(int countryId)
        {
            if (countryId > 0) 
            {
                var country = await _countryRepository.GetCountryWithCitiesAsync(countryId);
                return Json(country.Cities.OrderBy(c => c.Name));
            }

            return Json(Enumerable.Empty<City>());
        }

        // GET: Aeroportos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AirportNotFound");
            }

            var airport = await _airportRepository.GetByIdAsync(id.Value);

            if (airport == null)
            {
                return new NotFoundViewResult("AirportNotFound");
            }

            return View(airport);
        }

        // POST: Aeroportos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var airport = await _airportRepository.GetByIdAsync(id);

            try
            {
                var canDelete = await _airportService.AllowAirportDeletionOrUpdate(airport);

                if (canDelete)
                {
                    await _airportRepository.DeleteAsync(airport);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    var errorModel = new ErrorViewModel
                    {
                        ErrorTitle = $"{airport.Name} provavelmente está a ser usado!!",
                        ErrorMessage = $"{airport.Name} não pode ser apagado visto haverem encomendas que o usam.<br/><br/>" +
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

        public IActionResult AirportNotFound()
        {
            return View();
        }

        public object GetWorldMap()
        {
            string allText = System.IO.File.ReadAllText("./wwwroot/maps/worldmap.json");
            return JsonConvert.DeserializeObject(allText);
        }

    }
}
