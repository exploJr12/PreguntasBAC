using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Preguntas.Models;

namespace Preguntas.Controllers
{
    public class PreguntasController : Controller
    {
        private readonly PreguntasContext _context;

        public PreguntasController(PreguntasContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var preguntasContext = _context.Pregunta
                .Include(p => p.Usuario)
                .OrderByDescending(p => p.FechaCreacion)  // Ordenar por fecha de creación descendente (más reciente primero)
                .ToListAsync();

            return View(await preguntasContext);
        }


        // GET: Preguntas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pregunta = await _context.Pregunta
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.IdPregunta == id);
            if (pregunta == null)
            {
                return NotFound();
            }

            return View(pregunta);
        }

        // GET: Preguntas/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "IdUsuario", "Nombre");
            return View();
        }

        // POST: Preguntas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPregunta,Contenido,FechaCreacion,Cerrada,UsuarioId")] Pregunta pregunta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Guardar la pregunta en la base de datos
                    _context.Add(pregunta);
                    await _context.SaveChangesAsync();

                    // Redirigir al índice después de crear la pregunta
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error al guardar la pregunta: {ex.Message}");
                }
            }

            // Configurar ViewData con los datos necesarios para recargar la vista en caso de error
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "IdUsuario", "Nombre", pregunta.UsuarioId);
            return View(pregunta);
        }

        // GET: Preguntas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pregunta = await _context.Pregunta.FindAsync(id);
            if (pregunta == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "IdUsuario", "Nombre", pregunta.UsuarioId);
            return View(pregunta);
        }

        // POST: Preguntas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPregunta,Contenido,FechaCreacion,Cerrada,UsuarioId")] Pregunta pregunta)
        {
            if (id != pregunta.IdPregunta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pregunta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PreguntaExists(pregunta.IdPregunta))
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
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "IdUsuario", "Nombre", pregunta.UsuarioId);
            return View(pregunta);
        }


        // GET: Preguntas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pregunta = await _context.Pregunta
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.IdPregunta == id);
            if (pregunta == null)
            {
                return NotFound();
            }

            return View(pregunta);
        }

        // POST: Preguntas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pregunta = await _context.Pregunta.FindAsync(id);
            if (pregunta != null)
            {
                _context.Pregunta.Remove(pregunta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PreguntaExists(int id)
        {
            return _context.Pregunta.Any(e => e.IdPregunta == id);
        }
    }
}
