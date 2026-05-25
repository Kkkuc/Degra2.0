using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers;

public class TeachersController(ITeachersService teachersService) : Controller
{
    // GET: Teachers
    public async Task<IActionResult> Index()
    {
        var data = await teachersService.GetAllAsync();
        return View(data);
    }

    // GET: Teachers/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var teacher = await teachersService.GetByIdAsync(id.Value);
        if (teacher == null)
        {
            return NotFound();
        }

        return View(teacher);
    }

    // GET: Teachers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Teachers/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,AcademicTitle,FirstName,LastName,Email")] Teacher teacher)
    {
        if (!ModelState.IsValid)
        {
            return View(teacher);
        }

        await teachersService.CreateAsync(teacher);
        return RedirectToAction(nameof(Index));
    }

    // GET: Teachers/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var teacher = await teachersService.GetByIdAsync(id.Value);
        if (teacher == null)
        {
            return NotFound();
        }

        return View(teacher);
    }

    // POST: Teachers/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,AcademicTitle,FirstName,LastName,Email")] Teacher teacher)
    {
        if (id != teacher.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(teacher);
        }

        try
        {
            await teachersService.UpdateAsync(teacher);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await teachersService.ExistsAsync(teacher.Id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Teachers/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var teacher = await teachersService.GetByIdAsync(id.Value);
        if (teacher == null)
        {
            return NotFound();
        }

        return View(teacher);
    }

    // POST: Teachers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await teachersService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}