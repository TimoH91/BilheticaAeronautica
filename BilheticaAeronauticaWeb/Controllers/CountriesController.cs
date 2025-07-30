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
using Microsoft.AspNetCore.Authorization;
using BilheticaAeronauticaWeb.Helpers;

namespace BilheticaAeronauticaWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountriesController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IBlobHelper _blobHelper;

        public CountriesController(ICountryRepository countryRepository,
            IConverterHelper converterHelper,
            IBlobHelper blobHelper)
        {
            _countryRepository = countryRepository;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(_countryRepository.GetCountriesWithCities());
        }

        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("CityNotFound");
            }

            var city = await _countryRepository.GetCityAsync(id.Value);

            if (city == null)
            {
                return new NotFoundViewResult("CityNotFound");
            }

            var countryId = await _countryRepository.DeleteCityAsync(city);
            return this.RedirectToAction($"Details", new { id = countryId });
        }

        public async Task<IActionResult> EditCity(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("CityNotFound");
            }

            var city = await _countryRepository.GetCityAsync(id.Value);
            if (city == null)
            {
                return new NotFoundViewResult("CityNotFound");
            }

            return View(city);
        }


        [HttpPost]
        public async Task<IActionResult> EditCity(City city)
        {
            if (this.ModelState.IsValid)
            {
                var countryId = await _countryRepository.UpdateCityAsync(city);

                if (countryId != 0)
                {
                    return this.RedirectToAction($"Details", new { id = countryId });
                }
            }

            return this.View(city);
        }

        public async Task<IActionResult> AddCity(int? countryId)
        {
            if (countryId == null)
            {
                return new NotFoundViewResult("CityNotFound");
            }

            var country = await _countryRepository.GetByIdAsync(countryId.Value);

            if (country == null)
            {
                return new NotFoundViewResult("CountryNotFound");
            }

            var model = new CityViewModel { CountryId = country.Id };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCity(CityViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                await _countryRepository.AddCityAsync(model);

                return RedirectToAction("Index", new { id = model.CountryId });
            }

            return this.View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("CountryNotFound");
            }

            var country = await _countryRepository.GetCountryWithCitiesAsync(id.Value);

            if (country == null)
            {
                return new NotFoundViewResult("CountryNotFound");
            }

            return View(country);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CountryViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "countries");
                }

                var country = _converterHelper.ToCountry(model, imageId, true);

                try
                {
                    await _countryRepository.CreateAsync(country);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    //_flashMessage.Danger("This country already exists!");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("CountryNotFound");
            }

            var country = await _countryRepository.GetByIdAsync(id.Value);
            if (country == null)
            {
                return new NotFoundViewResult("CountryNotFound");
            }
            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                await _countryRepository.UpdateAsync(country);
                return RedirectToAction(nameof(Index));
            }

            return View(country);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("CountryNotFound");
            }

            var country = await _countryRepository.GetByIdAsync(id.Value);
            if (country == null)
            {
                return new NotFoundViewResult("CountryNotFound");
            }

            await _countryRepository.DeleteAsync(country);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CountryNotFound()
        {
            return View();
        }

        public IActionResult CityNotFound()
        {
            return View();
        }
    }
}
