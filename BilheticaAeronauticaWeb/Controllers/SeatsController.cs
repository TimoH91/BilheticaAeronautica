using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers
{
    [Authorize]
    public class SeatsController : Controller
    {
        private readonly ISeatRepository _seatRepository;

        public SeatsController(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        public IActionResult Index()
        {
            return View(_seatRepository.GetAll().OrderBy(a => a.Id));
        }
    }
}
