using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tesis.Models;

namespace Tesis.Models
{
    public partial class DbtesisContext : DbContext
    {
        public DbtesisContext()
        {
        }

        public DbtesisContext(DbContextOptions<DbtesisContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configurar eliminación en cascada para asistencias cuando se elimina un estudiante
            modelBuilder.Entity<Asistencias>()
                .HasOne(a => a.Estudiante)
                .WithMany(e => e.Asistencias) // Sin colección explícita de asistencias en Estudiantes
                .HasForeignKey(a => a.EstudianteId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Tesis.Models.Cursos> Cursos { get; set; } = default!;
        public DbSet<Tesis.Models.Estudiantes> Estudiantes { get; set; } = default!;
        public DbSet<Tesis.Models.Asistencias> Asistencias { get; set; } = default!;
    }
}
