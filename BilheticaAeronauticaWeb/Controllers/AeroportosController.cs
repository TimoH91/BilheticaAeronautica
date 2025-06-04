using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class AeroportosController : Controller
    {
        private readonly DataContext _context;
        private readonly IAeroportoRepository _aeroportoRepository;

        public AeroportosController(DataContext context, IAeroportoRepository aeroportoRepository)
        {
            _context = context;
            _aeroportoRepository = aeroportoRepository;
        }

        // GET: Aeroportos
        public async Task<IActionResult> Index()
        {
            return View(_aeroportoRepository.GetAll().OrderBy(a => a.Nome));
        }

        // GET: Aeroportos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aeroporto = await _aeroportoRepository.GetByIdAsync(id.Value);


            if (aeroporto == null)
            {
                return NotFound();
            }

            return View(aeroporto);
        }

        // GET: Aeroportos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Aeroportos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AeroportoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _aeroportoRepository.CreateAsync(model);

                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    //TODO Flashmessage"Este aeroporto já existe!"
                }
            }
            return View(model);
        }

        // GET: Aeroportos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AeroportoNotFound");
            }

            var aeroporto = await _aeroportoRepository.GetByIdAsync(id.Value);

            if (aeroporto == null)
            {
                return new NotFoundViewResult("AeroportoNotFound");
            }

            return View(aeroporto);
        }

        // POST: Aeroportos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AeroportoViewModel aeroporto)
        {
            if (ModelState.IsValid)
            {
                try
                {


                await _aeroportoRepository.UpdateAsync(aeroporto);


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _aeroportoRepository.ExistAsync(aeroporto.Id))
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
            return View(aeroporto);
        }

        // GET: Aeroportos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AeroportoNotFound");
            }

            var aeroporto = await _aeroportoRepository.GetByIdAsync(id.Value);

            if (aeroporto == null)
            {
                return NotFound();
            }

            return View(aeroporto);
        }

        // POST: Aeroportos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aeroporto = await _aeroportoRepository.GetByIdAsync(id);

            try
            {
                await _aeroportoRepository.DeleteAsync(aeroporto);
                return RedirectToAction(nameof(Index)); 
            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{aeroporto.Nome} provavelmente está a ser usado!!";
                    ViewBag.ErrorMessage = $"{aeroporto.Nome} não pode ser apagado visto haverem encomendas que o usam.</br></br>" +
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
