using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Preguntas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuración de la base de datos
            builder.Services.AddDbContext<PreguntasContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("PreguntasContext") ?? throw new InvalidOperationException("Connection string 'PreguntasContext' not found.")));

            // Agregar servicios de autenticación
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Usuarios/Login";  // Ruta para iniciar sesión
                    options.LogoutPath = "/Usuarios/CerrarSesion";  // Ruta para cerrar sesión
                });

            // Agregar servicios necesarios para controladores y vistas
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configurar el pipeline de la aplicación
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Asegurarse de que el middleware de enrutamiento esté antes
            app.UseRouting();

            // Asegurarse de que la autenticación esté antes de la autorización
            app.UseAuthentication(); // Agregar autenticación aquí
            app.UseAuthorization();  // Luego autorización

            // Configuración de las rutas
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Usuarios}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
