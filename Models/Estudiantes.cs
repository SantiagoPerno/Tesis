using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesis.Models
{
    public class Estudiantes
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obligatorio.")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obligatorio.")]
        public int DNI { get; set; }

        [Required(ErrorMessage = "Campo obligatorio.")]
        public int Legajo { get; set; }

        public string? Email { get; set; } 
        public string? Direccion { get; set; } 
        public int? Numero { get; set; } 
        public string? Telefono { get; set; } 

        public string? NombrePadre { get; set; }
        public string? TelefonoPadre { get; set; }
        public string? NombreMadre { get; set; }
        public string? TelefonoMadre { get; set; }
        public string? NombreTutor { get; set; }
        public string? TelefonoTutor { get; set; }

        [Required(ErrorMessage = "Seleccionar un curso.")]
        public int CursoId { get; set; }

        [ForeignKey("CursoId")]
        public Cursos Curso { get; set; } = null!;

        [NotMapped]
        public int TotalFaltas { get; set; }
        public ICollection<Asistencias> Asistencias { get; set; } = new List<Asistencias>();
    }


}
