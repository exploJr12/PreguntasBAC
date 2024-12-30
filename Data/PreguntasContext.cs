using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Preguntas.Models;

    public class PreguntasContext : DbContext
    {
        public PreguntasContext (DbContextOptions<PreguntasContext> options)
            : base(options)
        {
        }

        public DbSet<Preguntas.Models.Usuario> Usuario { get; set; } = default!;

public DbSet<Preguntas.Models.Pregunta> Pregunta { get; set; } = default!;

public DbSet<Respuesta> Respuesta { get; set; } = default!;
    }
