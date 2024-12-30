using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Preguntas.Models
{
    public class Usuario
    {
        [Key] // Clave primaria
        public int IdUsuario { get; set; }

        [Required] // Campo obligatorio
        [StringLength(50)] // Máximo 50 caracteres para el nombre de usuario
        public string Nombre { get; set; }

        [Required] // Campo obligatorio
        [StringLength(50)] // Máximo 50 caracteres para la contraseña
        public string Contraseña { get; set; }

        // Relación uno-a-muchos con Preguntas
        public List<Pregunta>? Preguntas { get; set; }
        public List<Respuesta>? Respuestas { get; set; }
    }
}
