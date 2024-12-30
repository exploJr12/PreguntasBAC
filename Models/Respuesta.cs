using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Preguntas.Models {
    public class Respuesta
    {
        [Key] // Llave primaria
        public int IdRespuesta { get; set; }

        [Required] // Campo obligatorio
        [ForeignKey("Pregunta")] // Llave foránea hacia la tabla Preguntas
        public int PreguntaId { get; set; }

        [Required] // Campo obligatorio
        [ForeignKey("Usuario")] // Llave foránea hacia la tabla Usuarios
        public int UsuarioId { get; set; }

        [Required] // Campo obligatorio
        public string Contenido { get; set; }

        [Required] // Campo obligatorio
        [Column(TypeName = "datetime")] // Especifica el tipo en SQL Server
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public Pregunta? Pregunta { get; set; } // Propiedad de navegación
        public Usuario? Usuario { get; set; } // Propiedad de navegación
    }
}

