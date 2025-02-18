using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tesis.Models;
using Tesis.ViewModels;

namespace Tesis.Controllers
{
    public class AsistenciasController : Controller
    {
        private readonly DbtesisContext _context;

        public AsistenciasController(DbtesisContext context)
        {
            _context = context;
        }

        //Guardar asistencia del estudiante en la BD

        [HttpPost]
        public async Task<IActionResult> GuardarAsistencias(List<EstudianteAsistenciaViewModel> Asistencias, string Fecha)
        {
            var fechaAsistencia = DateTime.Parse(Fecha).Date;

            foreach (var asistencia in Asistencias)
            {
                // Buscar si ya existe un registro de asistencia para el estudiante y la fecha
                var asistenciaExistente = await _context.Asistencias
                    .FirstOrDefaultAsync(a => a.EstudianteId == asistencia.Id && a.Fecha == fechaAsistencia);

                if (asistenciaExistente == null)
                {
                    // Crear nuevo registro si no existe
                    var nuevaAsistencia = new Asistencias
                    {
                        EstudianteId = asistencia.Id,
                        CursoId = (await _context.Estudiantes.FindAsync(asistencia.Id)).CursoId,
                        Fecha = fechaAsistencia,
                        Presente = asistencia.Presente // ✅ Guardar estado correcto
                    };
                    _context.Asistencias.Add(nuevaAsistencia);
                }
                else
                {
                    // Actualizar registro existente
                    asistenciaExistente.Presente = asistencia.Presente;
                    _context.Asistencias.Update(asistenciaExistente);
                }
            }

            // Guardar cambios en la base de datos
            await _context.SaveChangesAsync();

            // Redirigir a la vista de selección de cursos
            return RedirectToAction("Index", "Asistencias");
        }




        // GET: Asistencias/EstudiantesPorCurso/1

        public async Task<IActionResult> EstudiantesPorCurso(int id)
        {
            // Verifica que el curso existe
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
            {
                return NotFound();
            }

            // Obtiene los estudiantes y calcula asistencias e inasistencias
            var estudiantes = await _context.Estudiantes
                .Where(e => e.CursoId == id)
                .Select(e => new EstudianteAsistenciaViewModel
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    TotalAsistencias = _context.Asistencias.Count(a => a.EstudianteId == e.Id && a.Presente),
                    TotalInasistencias = _context.Asistencias.Count(a => a.EstudianteId == e.Id && !a.Presente)
                })
                .ToListAsync();

            ViewBag.CursoNombre = curso.Nombre;

            return View(estudiantes);
        }


        // GET: Asistencias
        public async Task<IActionResult> Index()
        {
            var cursos = await _context.Cursos
                .Include(c => c.Estudiantes) // ✅ Incluir estudiantes
                .ThenInclude(e => e.Asistencias) // ✅ Incluir asistencias de los estudiantes
                .ToListAsync();

            return View(cursos);
        }

        // GET: Asistencias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencias = await _context.Asistencias
                .Include(a => a.Curso)
                .Include(a => a.Estudiante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asistencias == null)
            {
                return NotFound();
            }

            return View(asistencias);
        }

        // GET: Asistencias/Create
        public IActionResult Create()
        {
            ViewData["CursoId"] = new SelectList(_context.Cursos, "Id", "Id");
            ViewData["EstudianteId"] = new SelectList(_context.Estudiantes, "Id", "Nombre");
            return View();
        }

        // POST: Asistencias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CursoId,EstudianteId,Fecha,Presente")] Asistencias asistencias)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asistencias);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CursoId"] = new SelectList(_context.Cursos, "Id", "Id", asistencias.CursoId);
            ViewData["EstudianteId"] = new SelectList(_context.Estudiantes, "Id", "Nombre", asistencias.EstudianteId);
            return View(asistencias);
        }

        // GET: Asistencias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencias = await _context.Asistencias.FindAsync(id);
            if (asistencias == null)
            {
                return NotFound();
            }
            ViewData["CursoId"] = new SelectList(_context.Cursos, "Id", "Id", asistencias.CursoId);
            ViewData["EstudianteId"] = new SelectList(_context.Estudiantes, "Id", "Nombre", asistencias.EstudianteId);
            return View(asistencias);
        }

        // POST: Asistencias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CursoId,EstudianteId,Fecha,Presente")] Asistencias asistencias)
        {
            if (id != asistencias.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asistencias);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsistenciasExists(asistencias.Id))
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
            ViewData["CursoId"] = new SelectList(_context.Cursos, "Id", "Id", asistencias.CursoId);
            ViewData["EstudianteId"] = new SelectList(_context.Estudiantes, "Id", "Nombre", asistencias.EstudianteId);
            return View(asistencias);
        }

        // GET: Asistencias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencias = await _context.Asistencias
                .Include(a => a.Curso)
                .Include(a => a.Estudiante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asistencias == null)
            {
                return NotFound();
            }

            return View(asistencias);
        }

        // POST: Asistencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asistencias = await _context.Asistencias.FindAsync(id);
            if (asistencias != null)
            {
                _context.Asistencias.Remove(asistencias);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AsistenciasExists(int id)
        {
            return _context.Asistencias.Any(e => e.Id == id);
        }
    }
}
