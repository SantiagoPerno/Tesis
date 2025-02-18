using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Tesis.Models;

namespace Tesis.Controllers
{
    public class EstudiantesController : Controller
    {
        private readonly DbtesisContext _context;

        public EstudiantesController(DbtesisContext context)
        {
            _context = context;
        }
       
        //Eliminar seleccion de estudiantes
        [HttpPost]
        public async Task<IActionResult> EliminarSeleccionados(int[] selectedEstudiantes)
        {
            if (selectedEstudiantes != null && selectedEstudiantes.Length > 0)
            {
                var estudiantes = _context.Estudiantes.Where(e => selectedEstudiantes.Contains(e.Id)).ToList();
                _context.Estudiantes.RemoveRange(estudiantes);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Importar Excel
        [HttpPost]
        public async Task<IActionResult> ImportarEstudiantes(IFormFile archivoExcel)
        {
            if (archivoExcel == null || archivoExcel.Length == 0)
            {
                TempData["Error"] = "Por favor, seleccione un archivo válido.";
                return RedirectToAction(nameof(Index));
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await archivoExcel.CopyToAsync(stream);


                using (var package = new ExcelPackage(stream))
                {

                    int totalHojas = package.Workbook.Worksheets.Count;

                    for (int hojaIndex = 0; hojaIndex < totalHojas; hojaIndex++)
                    {
                        var worksheet = package.Workbook.Worksheets[hojaIndex];
                        int rowCount = worksheet.Dimension?.Rows ?? 0; // Manejo de hoja vacía


                        for (int row = 2; row <= rowCount; row++) // empieza en fila 2 xq la 1 tiene los encabezados
                        {
                            //recorre cada fila por celda (fila, columa)
                            var nombre = worksheet.Cells[row, 1].Value?.ToString();
                            var apellido = worksheet.Cells[row, 2].Value?.ToString();
                            var dni = int.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out int dniParsed) ? dniParsed : 0;
                            var legajo = int.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out int legajoParsed) ? legajoParsed : 0;
                            var email = worksheet.Cells[row, 5].Value?.ToString();
                            var direccion = worksheet.Cells[row, 6].Value?.ToString();
                            var altura = int.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out int alturaParsed) ? alturaParsed : 0;
                            var telefono = worksheet.Cells[row, 8].Value?.ToString();
                            var nombrePadre = worksheet.Cells[row, 9].Value?.ToString();
                            var telefonoPadre = worksheet.Cells[row, 10].Value?.ToString();
                            var nombreMadre = worksheet.Cells[row, 11].Value?.ToString();
                            var telefonoMadre = worksheet.Cells[row, 12].Value?.ToString();
                            var nombreTutor = worksheet.Cells[row, 13].Value?.ToString();
                            var telefonoTutor = worksheet.Cells[row, 14].Value?.ToString();
                            var cursoNombre = worksheet.Cells[row, 15].Value?.ToString(); // Se espera que el curso venga con su nombre

                            // Buscar el curso en la BD
                            var curso = await _context.Cursos.FirstOrDefaultAsync(c => c.Nombre == cursoNombre);
                            if (curso == null)
                            {
                                continue; // Si no existe el curso, omitir este estudiante
                            }

                            var nuevoEstudiante = new Estudiantes
                            {
                                Nombre = nombre ?? "Sin Nombre",
                                Apellido = apellido ?? "Sin Apellido",
                                DNI = dni,
                                Legajo = legajo,
                                Email = email,
                                Direccion = direccion,
                                Numero = altura,
                                Telefono = telefono,
                                NombrePadre = nombrePadre,
                                TelefonoPadre = telefonoMadre,
                                NombreMadre = nombreMadre,
                                TelefonoMadre = nombreMadre,
                                NombreTutor = nombreTutor,
                                TelefonoTutor = telefonoTutor,
                                CursoId = curso.Id
                            };

                            _context.Estudiantes.Add(nuevoEstudiante);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }

            TempData["Success"] = "Importación exitosa.";
            return RedirectToAction(nameof(Index));
        }


        // GET: Estudiantes
        public async Task<IActionResult> Index(string searchString)
        {
            var estudiantes = _context.Estudiantes
                .Include(e => e.Curso) // Incluimos el curso para poder filtrar
                .AsQueryable(); // Permite modificar la consulta dinámicamente

            if (!string.IsNullOrEmpty(searchString))
            {
                estudiantes = estudiantes.Where(e =>
                    e.Nombre.Contains(searchString) ||
                    e.Apellido.Contains(searchString) ||  // Asegúrate de tener este atributo en tu modelo
                    e.Curso.Nombre.Contains(searchString) // Buscar por nombre del curso
                );
            }

            return View(await estudiantes.ToListAsync());
        }

        // GET: Estudiantes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estudiantes = await _context.Estudiantes
                .Include(e => e.Curso)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estudiantes == null)
            {
                return NotFound();
            }

            return View(estudiantes);
        }

        // GET: Estudiantes/Create
        public IActionResult Create()
        {
            ViewData["CursoId"] = new SelectList(_context.Cursos, "Id", "Nombre");
            return View();
        }

        // POST: Estudiantes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Apellido,DNI,Legajo,Email,Direccion,Numero,Telefono,NombrePadre,TelefonoPadre,NombreMadre,TelefonoMadre,NombreTutor,TelefonoTutor,CursoId")] Estudiantes estudiantes)
        {
            if (ModelState.IsValid)
            {
                ViewData["CursoId"] = new SelectList(_context.Cursos, "Id", "Nombre", estudiantes.CursoId);
                return View(estudiantes);

            }
            _context.Add(estudiantes);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Estudiantes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante == null)
            {
                return NotFound();
            }

            // ✅ Cargar correctamente los cursos en ViewBag
            ViewBag.CursoId = new SelectList(await _context.Cursos.ToListAsync(), "Id", "Nombre", estudiante.CursoId);

            return View(estudiante);
        }


        // POST: Estudiantes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Estudiantes estudiantes)
        {

            if (id != estudiantes.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(estudiantes);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstudiantesExists(estudiantes.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }

            ViewBag.CursoId = new SelectList(await _context.Cursos.ToListAsync(), "Id", "Nombre", estudiantes.CursoId);

            return View(estudiantes);
        }

        // GET: Estudiantes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estudiantes = await _context.Estudiantes
                .Include(e => e.Curso)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estudiantes == null)
            {
                return NotFound();
            }

            return View(estudiantes);
        }

        // POST: Estudiantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estudiante = await _context.Estudiantes
                .Include(e => e.Asistencias) // ✅ Incluir asistencias
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estudiante == null)
            {
                return NotFound();
            }

            // 🔥 Eliminar asistencias antes de eliminar el estudiante (solo si sigue sin funcionar)
            _context.Asistencias.RemoveRange(estudiante.Asistencias);

            _context.Estudiantes.Remove(estudiante);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool EstudiantesExists(int id)
        {
            return _context.Estudiantes.Any(e => e.Id == id);
        }
    }
}
