using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Preguntas.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Preguntas.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly PreguntasContext _context;

        public UsuariosController(PreguntasContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuario.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,Nombre,Contraseña")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe un usuario con el mismo nombre
                var usuarioExistente = await _context.Usuario
                    .FirstOrDefaultAsync(u => u.Nombre == usuario.Nombre);

                if (usuarioExistente != null)
                {
                    // Si el usuario ya existe, devolver un mensaje de error
                    ViewBag.Error = "El nombre de usuario ya está en uso.";
                    return View(usuario);  // Volver a la vista de creación con el mensaje de error
                }

                // Crear el nuevo usuario
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                // Redirigir al Login después de la creación
                return RedirectToAction("Login");
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,Nombre,Contraseña")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.IdUsuario))
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
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.IdUsuario == id);
        }

        // Métodos para login

        // GET: Login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.Url = returnUrl;
            ViewBag.Error = "";
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Usuario usuario, string returnUrl = null)
        {
            try
            {
                // Verificar si el usuario existe y las credenciales son correctas
                var usuarioAutenticado = await _context.Usuario
                    .FirstOrDefaultAsync(u => u.Nombre == usuario.Nombre && u.Contraseña == usuario.Contraseña);

                if (usuarioAutenticado != null)
                {
                    // Crear las reclamaciones para la sesión
                    var claims = new[] { new Claim(ClaimTypes.Name, usuarioAutenticado.Nombre) };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Iniciar sesión
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    if (!string.IsNullOrWhiteSpace(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Preguntas");
                    }
                }
                else
                {
                    throw new Exception("Credenciales incorrectas");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Url = returnUrl;
                return View();
            }
        }

        // GET: Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Cambiar contraseña
        [Authorize]
        public IActionResult CambiarPassword()
        {
            var usuarioActual = _context.Usuario.FirstOrDefault(u => u.Nombre == User.Identity.Name);
            return View(usuarioActual);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CambiarPassword(Usuario usuario, string passwordAnt)
        {
            try
            {
                var usuarioDb = await _context.Usuario.FindAsync(usuario.IdUsuario);
                if (usuarioDb != null && usuarioDb.Contraseña == passwordAnt)
                {
                    usuarioDb.Contraseña = usuario.Contraseña;
                    _context.Update(usuarioDb);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    throw new Exception("Contraseña incorrecta");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(usuario);
            }
        }
    }
}
