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
using System.Diagnostics;
using BilheticaAeronauticaWeb.Models;
using BilheticaAeronauticaWeb.Migrations;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using BilheticaAeronauticaWeb.Services;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;

namespace BilheticaAeronauticaWeb.Controllers
{
    
    public class TicketsController : Controller
    {
        private readonly DataContext _context;
        private readonly ITicketRepository _ticketRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IAirportRepository _airportRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly ITicketService _ticketService;



        public TicketsController(DataContext context, 
            ITicketRepository ticketRepository, 
            IFlightRepository flightRepository,
            IAirportRepository airportRepository,
            ISeatRepository seatRepository,
            IConverterHelper converterHelper,
            IUserHelper userHelper,
            ITicketService ticketService)
        {
            _context = context;
            _ticketRepository = ticketRepository;
            _flightRepository = flightRepository;
            _airportRepository = airportRepository;
            _seatRepository = seatRepository;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _ticketService = ticketService;
        }

        [Authorize(Roles = "Staff")]
        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            return View(_ticketRepository.GetAll().OrderBy(a => a.Id));
        }

        //[Authorize(Roles = "Staff")]
        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            var ticket = await _ticketRepository.GetByIdAsync(id.Value);


            if (ticket == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            if (User.Identity.Name != null)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                var roles = await _userHelper.GetRolesAsync(user);

                if (roles.Contains("Admin") || roles.Contains("Staff"))
                {
                    //TODO: change this page
                    return Forbid();
                }
            }

            ViewBag.Airports = _airportRepository.GetComboAirports();

            return View(new TicketViewModel());
        }

        public async Task<IActionResult> CreateSpecificTicket(int? id)
        {
            if (id != null)
            {
                var flight = await _flightRepository.GetByIdAsync(id.Value);

                var model = new TicketViewModel 
                {
                    Id = 0,
                    Name = "John",
                    Surname = "Smith",
                    FlightId = flight.Id,
                    OriginAirportId = flight.OriginAirportId,
                    DestinationAirportId = flight.DestinationAirportId,
                    Class = TicketClass.Economic,
                    Price = flight.BasePrice,
                    Type = Entities.PassengerType.Adult,
                };


                ViewBag.Origin = flight.OriginAirport.Name;
                ViewBag.Destination = flight.DestinationAirport.Name;
                ViewBag.Layover = flight.Layover?.Name;


                return View(model);
            }



            return RedirectToAction("Index");
        }

            [HttpPost]
        [Route("Tickets/GetFlightsByRoute")]
        public async Task<JsonResult> GetFlightsByRoute(int originAirportId, int destinationAirportId)
        {
            if (originAirportId > 0 && destinationAirportId > 0)
            {
                var flights = await _flightRepository.GetFlightsByOriginAndDestination(originAirportId, destinationAirportId);

                var flightsListItems = flights.Select(flight => new
                {
                    id = flight.Id,
                    name = flight.OriginAirport.Name + "to" + flight.DestinationAirport.Name
                }).ToList();

                if (flightsListItems != null)
                {
                    return Json(flightsListItems);
                }

            }

            return Json(Enumerable.Empty<object>());
        }

        [HttpPost]
        [Route("Tickets/GetSeatsByFlight")]
        public async Task<JsonResult> GetSeatsByFlight(int flightId)
        {
            if (flightId > 0)
            {
                var seats = await _seatRepository.GetAvailableSeatsByFlight(flightId);

                var seatsAdjusted = await _ticketService.UnholdSeats(seats);

                var seatsFiltered = _ticketService.RemoveHeldSeats(seatsAdjusted);

                var selectListItems = seatsFiltered.Select(seat => new
                {
                    value = seat.Id,          
                    text = "Row " + seat.Row + " Seat " + seat.Column 
                }).ToList();

                if (selectListItems != null)
                {
                    return Json(selectListItems);
                }
            }

            return Json(Enumerable.Empty<SelectListItem>());
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.FlightId != null)
                {
                    var flight = await _flightRepository.GetByIdAsync(model.FlightId.Value);

                    var ticket = _converterHelper.ToTicket(model, true, flight);

                    try
                    {
                        await _ticketRepository.CreateAsync(ticket);
                        //await _flightService.CreateSeatsForFlightAsync(flight);

                        return RedirectToAction(nameof(Index));

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }

            return View(model);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            var ticket = await _ticketRepository.GetByIdAsync(id.Value);

            if (ticket == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            var model = ConvertToModel(ticket);

            if (ticket.Flight != null)
            {
                ViewBag.Origin = ticket.Flight.OriginAirport.Name;
                ViewBag.Destination = ticket.Flight.DestinationAirport.Name;
            }

            return View(model);
        }

        private TicketViewModel ConvertToModel(Ticket ticket)
        {
            if (ticket is InfantTicket infantTicket)
            {
               return _converterHelper.ToInfantTicketViewModel(infantTicket);
            }

            return _converterHelper.ToAdultAndChildTicketViewModel(ticket);

        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                var canEdit = await _ticketService.AllowTicketChanges(model);

                if (model.FlightId != null && canEdit)
                {
                    try
                    {
                        var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                        var flight = await _flightRepository.GetByIdAsync(model.FlightId.Value);

                        var ticket = _converterHelper.ToTicket(model, false, flight);

                        ticket.UserId = user.Id;

                        await _ticketRepository.UpdateAsync(ticket);
                    }
                    catch (DbUpdateConcurrencyException)
                    {

                        if (!await _ticketRepository.ExistAsync(model.Id))
                        {
                            return new NotFoundViewResult("TicketNotFound"); ;
                        }
                        else
                        {
                            throw;
                        }

                    }
                    return RedirectToAction("Index", "Orders");
                }
            }
            return View(model);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            var ticket = await _ticketRepository.GetByIdAsync(id.Value);

            if (ticket == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);

            try
            {
                await _ticketRepository.DeleteAsync(ticket);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    var errorModel = new ErrorViewModel
                    {
                        ErrorTitle = $"{ticket.Id} provavelmente está a ser usado!!",
                        ErrorMessage = $"{ticket.Id} não pode ser apagado visto haverem encomendas que o usam.<br/><br/>" +
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
