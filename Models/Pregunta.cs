using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Preguntas.Models
{
    public class Pregunta
    {
        [Key] // Clave primaria
        public int IdPregunta { get; set; }

        [Required] // Campo obligatorio
        [MaxLength(500)] // Máximo 500 caracteres para el contenido
        public string Contenido { get; set; }

        [Required] // Campo obligatorio
        public DateTime FechaCreacion { get; set; } = DateTime.Now; // Valor por defecto

        [Required] // Campo obligatorio
        public bool Cerrada { get; set; } = false; // Valor por defecto

        [ForeignKey(nameof(Usuario))] // Llave foránea hacia Usuario
        public int UsuarioId { get; set; }

        public Usuario? Usuario { get; set; } // Propiedad de navegación opcional
        public List<Respuesta>? Respuestas { get; set; }
    }

}
