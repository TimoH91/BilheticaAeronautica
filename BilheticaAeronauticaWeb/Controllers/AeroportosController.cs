using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class AeroportosController : Controller
    {
        private readonly DataContext _context;

        public AeroportosController(DataContext context)
        {
            _context = context;
        }

        // GET: Aeroportos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Aeroportos.ToListAsync());
        }

        // GET: Aeroportos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aeroporto = await _context.Aeroportos
                .FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> Create([Bind("Id,Nome,Cidade,Pais")] Aeroporto aeroporto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aeroporto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aeroporto);
        }

        // GET: Aeroportos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aeroporto = await _context.Aeroportos.FindAsync(id);
            if (aeroporto == null)
            {
                return NotFound();
            }
            return View(aeroporto);
        }

        // POST: Aeroportos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Cidade,Pais")] Aeroporto aeroporto)
        {
            if (id != aeroporto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aeroporto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AeroportoExists(aeroporto.Id))
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
                return NotFound();
            }

            var aeroporto = await _context.Aeroportos
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var aeroporto = await _context.Aeroportos.FindAsync(id);
            if (aeroporto != null)
            {
                _context.Aeroportos.Remove(aeroporto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AeroportoExists(int id)
        {
            return _context.Aeroportos.Any(e => e.Id == id);
        }
    }
}
