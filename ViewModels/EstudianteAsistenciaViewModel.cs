namespace Tesis.ViewModels
{
    public class EstudianteAsistenciaViewModel
    {
        public int Id { get; set; } // ID del estudiante
        public string Nombre { get; set; } = string.Empty; // Nombre del estudiante
        public int TotalAsistencias { get; set; } // Total de asistencias
        public int TotalInasistencias { get; set; } // Total de inasistencias
        public bool Presente { get; set; }
        public object? CursoId { get; set; }
    }
}
