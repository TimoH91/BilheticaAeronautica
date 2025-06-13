using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Models;
using System.Diagnostics;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class FlightsController : Controller
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IAirplaneRepository _airplaneRepository;
        private readonly IAirportRepository _airportRepository;
        private readonly IConverterHelper _converterHelper;

        public FlightsController(IFlightRepository flightRepository, IAirplaneRepository airplaneRepository, IAirportRepository airportRepository, IConverterHelper converterHelper)
        {
            _flightRepository = flightRepository;
            _airplaneRepository = airplaneRepository;
            _airportRepository = airportRepository;
            _converterHelper = converterHelper;

        }

        // GET: Flights
        public async Task<IActionResult> Index()
        {
            return View(_flightRepository.GetAll().OrderBy(a => a.Id));
        }

        // GET: Flights/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var airport = await _flightRepository.GetByIdAsync(id.Value);


            if (airport == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            return View(airport);
        }

        // GET: Flights/Create
        public IActionResult Create()
        {
            ViewBag.Airplanes = _airplaneRepository.GetComboAirplanes();
            ViewBag.Airports = _airportRepository.GetComboAirports();

            return View();
        }

        // POST: Flights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlightViewModel model)
        {
            if (ModelState.IsValid)
            {
                var flight = await _converterHelper.ToFlight(model, true);

                try
                {
                    await _flightRepository.CreateAsync(flight);

                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            return View(model);
        }

        // GET: Flights/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var flight = await _context.Flights.FindAsync(id);
            //if (flight == null)
            //{
            //    return NotFound();
            //}
            //return View(flight);
        //}

        // POST: Flights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Time,Duration,BasePrice")] Flight flight)
        //{
            //if (id != flight.Id)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(flight);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!FlightExists(flight.Id))
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
            //return View(flight);
        }

        // GET: Flights/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var flight = await _context.Flights
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (flight == null)
            //{
            //    return NotFound();
            //}

            //return View(flight);
        //}

        // POST: Flights/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    //var flight = await _context.Flights.FindAsync(id);
        //    //if (flight != null)
        //    //{
        //    //    _context.Flights.Remove(flight);
        //    //}

        //    //await _context.SaveChangesAsync();
        //    //return RedirectToAction(nameof(Index));
        //}

        //private bool FlightExists(int id)
        //{
        //    //return _context.Flights.Any(e => e.Id == id);
        //}
    //}
}
