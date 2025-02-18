using static Tesis.Models.Estudiantes;

namespace Tesis.Models
{
    public class Cursos
    {
        public int Id { get; set; } // Id del curso
        public string Nombre { get; set; } = string.Empty; // Nombre del curso
        public ICollection<Estudiantes> Estudiantes { get; set; } = new List<Estudiantes>(); // Relación uno a muchos
    }

}
