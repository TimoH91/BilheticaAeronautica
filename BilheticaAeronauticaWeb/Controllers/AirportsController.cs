using System.Runtime.InteropServices;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class AirportsController : Controller
    {
        private readonly DataContext _context;
        private readonly IAirportRepository _airportRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IConverterHelper _converterHelper;
        //private readonly IBlobHelper _blobHelper;

        public AirportsController(DataContext context, IAirportRepository aeroportoRepository, IConverterHelper converterHelper, ICountryRepository countryRepository)
        {
            _context = context;
            _airportRepository = aeroportoRepository;
            _converterHelper = converterHelper;
            _countryRepository = countryRepository;
            //_blobHelper = blobHelper;
        }

        // GET: Airports
        public async Task<IActionResult> Index()
        {
            return View(_airportRepository.GetAll().OrderBy(a => a.Name));
        }

        // GET: Airports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airport = await _airportRepository.GetByIdAsync(id.Value);


            if (airport == null)
            {
                return NotFound();
            }

            return View(airport);
        }

        // GET: Airports/Create
        public IActionResult Create()
        {
            ViewBag.Countries = _countryRepository.GetComboCountries();

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

            return View(airport);
        }

        // POST: Airports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AirportViewModel airport)
        {
            //if (ModelState.IsValid)
            //{
            //    try
            //    {


            //    await _airportRepository.UpdateAsync(airport);


            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!await _airportRepository.ExistAsync(airport.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}
            return View(airport);
        }

        [HttpPost]
        [Route("Airports/GetCitiesAsync")]
        public async Task<JsonResult> GetCitiesAsync(int countryId)
        {
            var country = await _countryRepository.GetCountryWithCitiesAsync(countryId);
            return Json(country.Cities.OrderBy(c => c.Name));
        }

        // GET: Aeroportos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AeroportoNotFound");
            }

            var airport = await _airportRepository.GetByIdAsync(id.Value);

            if (airport == null)
            {
                return NotFound();
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
                await _airportRepository.DeleteAsync(airport);
                return RedirectToAction(nameof(Index)); 
            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{airport.Name} provavelmente está a ser usado!!";
                    ViewBag.ErrorMessage = $"{airport.Name} não pode ser apagado visto haverem encomendas que o usam.</br></br>" +
                        $"Experimente primeiro apagar todas as encomendas que o estão a usar," +
                        $"e torne novamente a apagá-lo";
                }

                return View("Error");
            }
        }

        public IActionResult AeroportoNotFound()
        {
            return View();
        }
    }
}
