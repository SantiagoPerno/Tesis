using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Tesis.Models;
using Tesis.ViewModels;

namespace Tesis.Controllers
{
    public class CursosController : Controller
    {
        private readonly DbtesisContext _context;

        public CursosController(DbtesisContext context)
        {
            _context = context;
        }

        //Exportar excel

        public async Task<IActionResult> ExportarEstudiantes(int id)
        {
            var curso = await _context.Cursos
                .Include(c => c.Estudiantes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null || curso.Estudiantes.Count == 0)
            {
                TempData["Error"] = "No hay estudiantes para exportar.";
                return RedirectToAction(nameof(ListaEstudiantes), new { id });
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add($"Estudiantes de {curso.Nombre}");

                // ✅ Establecer la primera fila con el nombre del curso
                worksheet.Cells["A1"].Value = $"Estudiantes de {curso.Nombre}";
                worksheet.Cells["A1:N1"].Merge = true; // Fusionar celdas
                worksheet.Cells["A1"].Style.Font.Size = 14;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // ✅ Segunda fila: Encabezados
                var headers = new string[]
                {
            "Nombre", "Apellido", "DNI", "Legajo", "Email", "Dirección", "Altura",
            "Padre", "Tel padre", "Madre", "Tel madre", "Tutor", "Tel tutor"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[2, i + 1].Value = headers[i];
                    worksheet.Cells[2, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[2, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[2, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                }

                // ✅ Rellenar los datos de los estudiantes (Desde la fila 3 en adelante)
                int fila = 3;
                foreach (var estudiante in curso.Estudiantes)
                {
                    worksheet.Cells[fila, 1].Value = estudiante.Nombre;
                    worksheet.Cells[fila, 2].Value = estudiante.Apellido;
                    worksheet.Cells[fila, 3].Value = estudiante.DNI;
                    worksheet.Cells[fila, 4].Value = estudiante.Legajo;
                    worksheet.Cells[fila, 5].Value = estudiante.Email;
                    worksheet.Cells[fila, 6].Value = estudiante.Direccion;
                    worksheet.Cells[fila, 7].Value = estudiante.Numero;
                    worksheet.Cells[fila, 8].Value = estudiante.NombrePadre;
                    worksheet.Cells[fila, 9].Value = estudiante.TelefonoPadre;
                    worksheet.Cells[fila, 10].Value = estudiante.NombreMadre;
                    worksheet.Cells[fila, 11].Value = estudiante.TelefonoMadre;
                    worksheet.Cells[fila, 12].Value = estudiante.NombreTutor;
                    worksheet.Cells[fila, 13].Value = estudiante.TelefonoTutor;
                    fila++;
                }

                // ✅ Ajustar el tamaño de las columnas
                worksheet.Cells.AutoFitColumns();

                // ✅ Convertir el archivo en memoria y descargarlo
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"Estudiantes_{curso.Nombre.Replace(" ", "_")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        // GET: Cursos/ListaEstudiantes
        public async Task<IActionResult> ListaEstudiantes(int id)
        {
            var curso = await _context.Cursos
                .Include(c => c.Estudiantes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null)
            {
                return NotFound();
            }

            foreach (var estudiantes in curso.Estudiantes)
            {
                estudiantes.TotalFaltas = await _context.Asistencias
                    .Where(a => a.EstudianteId == estudiantes.Id && !a.Presente)
                    .CountAsync();
            }
            return View(curso);
        }

        // GET: Cursos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cursos.ToListAsync());
        }

        // GET: Cursos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cursos = await _context.Cursos
                .Include(c => c.Estudiantes)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (cursos == null)
            {
                return NotFound();
            }

            return View(cursos);
        }

        // GET: Cursos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cursos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] Cursos cursos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cursos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cursos);
        }

        // GET: Cursos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cursos = await _context.Cursos.FindAsync(id);
            if (cursos == null)
            {
                return NotFound();
            }
            return View(cursos);
        }

        // POST: Cursos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] Cursos cursos)
        {
            if (id != cursos.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cursos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CursosExists(cursos.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cursos);
        }

        // GET: Cursos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cursos = await _context.Cursos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cursos == null)
            {
                return NotFound();
            }

            return View(cursos);
        }

        // POST: Cursos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cursos = await _context.Cursos.FindAsync(id);
            if (cursos != null)
            {
                _context.Cursos.Remove(cursos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CursosExists(int id)
        {
            return _context.Cursos.Any(e => e.Id == id);
        }
    }
}
