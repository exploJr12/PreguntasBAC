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
    public class RespuestasController : Controller
    {
        private readonly PreguntasContext _context;

        public RespuestasController(PreguntasContext context)
        {
            _context = context;
        }

        // GET: Respuestas
        public async Task<IActionResult> Index(int? preguntaId)
        {
            if (preguntaId == null)
            {
                TempData["Error"] = "No se especificó una pregunta válida.";
                return RedirectToAction("Index", "Preguntas");
            }

            // Obtener la pregunta específica
            var pregunta = await _context.Pregunta.FindAsync(preguntaId);
            if (pregunta == null)
            {
                TempData["Error"] = "La pregunta no existe.";
                return RedirectToAction("Index", "Preguntas");
            }

            // Obtener las respuestas relacionadas
            var respuestas = await _context.Respuesta
                .Include(r => r.Pregunta)
                .Include(r => r.Usuario)
                .Where(r => r.PreguntaId == preguntaId) // Obtener respuestas de esta pregunta
                .OrderByDescending(r => r.FechaCreacion) // Ordenar por fecha de creación
                .ToListAsync();

            // Pasar la información de la pregunta a la vista
            ViewData["Pregunta"] = pregunta;

            return View(respuestas);
        }



        // GET: Respuestas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var respuesta = await _context.Respuesta
                .Include(r => r.Pregunta)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.IdRespuesta == id);
            if (respuesta == null)
            {
                return NotFound();
            }

            return View(respuesta);
        }

        // GET: Respuestas/Create
        public async Task< IActionResult> Create()
        {
            // Obtener solo preguntas abiertas para el dropdown
            var preguntasAbiertas = _context.Pregunta.Where(p => !p.Cerrada).ToList();
            ViewData["PreguntaId"] = new SelectList(preguntasAbiertas, "IdPregunta", "Contenido");

            // Obtener la lista de usuarios
            var usuarios = _context.Usuario.ToList();
            ViewData["UsuarioId"] = new SelectList(usuarios, "IdUsuario", "Nombre");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PreguntaId,UsuarioId,Contenido")] Respuesta respuesta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Asignar fecha de creación automáticamente
                    respuesta.FechaCreacion = DateTime.Now;

                    // Agregar la respuesta al contexto
                    _context.Add(respuesta);
                    await _context.SaveChangesAsync();

                    // Redirigir al listado de respuestas de la pregunta específica
                    return RedirectToAction("Index", new { preguntaId = respuesta.PreguntaId });
                }
                catch (Exception ex)
                {
                    // Agregar un mensaje de error a ModelState si ocurre una excepción
                    ModelState.AddModelError(string.Empty, $"Error al guardar la respuesta: {ex.Message}");
                }
            }

            // Si el modelo no es válido o hay un error, recargar listas desplegables
            var preguntasAbiertas = _context.Pregunta.Where(p => !p.Cerrada).ToList();
            ViewData["PreguntaId"] = new SelectList(preguntasAbiertas, "IdPregunta", "Contenido", respuesta.PreguntaId);

            var usuarios = _context.Usuario.ToList();
            ViewData["UsuarioId"] = new SelectList(usuarios, "IdUsuario", "Nombre", respuesta.UsuarioId);

            // Devolver la vista con los datos ingresados por el usuario
            return View(respuesta);
        }





        // GET: Respuestas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var respuesta = await _context.Respuesta.FindAsync(id);
            if (respuesta == null)
            {
                return NotFound();
            }
            ViewData["PreguntaId"] = new SelectList(_context.Pregunta, "IdPregunta", "Contenido", respuesta.PreguntaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "IdUsuario", "Nombre", respuesta.UsuarioId);
            return View(respuesta);
        }

        // POST: Respuestas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRespuesta,PreguntaId,UsuarioId,Contenido,FechaCreacion")] Respuesta respuesta)
        {
            if (id != respuesta.IdRespuesta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(respuesta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RespuestaExists(respuesta.IdRespuesta))
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
            ViewData["PreguntaId"] = new SelectList(_context.Pregunta, "IdPregunta", "Contenido", respuesta.PreguntaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "IdUsuario", "Nombre", respuesta.UsuarioId);
            return View(respuesta);
        }

        // GET: Respuestas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var respuesta = await _context.Respuesta
                .Include(r => r.Pregunta)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.IdRespuesta == id);
            if (respuesta == null)
            {
                return NotFound();
            }

            return View(respuesta);
        }

        // POST: Respuestas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var respuesta = await _context.Respuesta.FindAsync(id);
            if (respuesta != null)
            {
                _context.Respuesta.Remove(respuesta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RespuestaExists(int id)
        {
            return _context.Respuesta.Any(e => e.IdRespuesta == id);
        }
    }
}
