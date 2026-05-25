using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers;

public class SubjectsController(ISubjectsService subjectsService) : Controller
{
    // GET: Subjects
    public async Task<IActionResult> Index()
    {
        var data = await subjectsService.GetAllAsync();
        return View(data);
    }

    // GET: Subjects/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var subject = await subjectsService.GetByIdAsync(id.Value);
        if (subject == null)
        {
            return NotFound();
        }

        return View(subject);
    }

    // GET: Subjects/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Subjects/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Abbreviation,Code")] Subject subject)
    {
        if (ModelState.IsValid)
        {
            await subjectsService.CreateAsync(subject);
            return RedirectToAction(nameof(Index));
        }
        return View(subject);
    }

    // GET: Subjects/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var subject = await subjectsService.GetByIdAsync(id.Value);
        if (subject == null)
        {
            return NotFound();
        }
        return View(subject);
    }

    // POST: Subjects/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Abbreviation,Code")] Subject subject)
    {
        if (id != subject.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await subjectsService.UpdateAsync(subject);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await subjectsService.ExistsAsync(subject.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(subject);
    }

    // GET: Subjects/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var subject = await subjectsService.GetByIdAsync(id.Value);
        if (subject == null)
        {
            return NotFound();
        }

        return View(subject);
    }

    // POST: Subjects/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await subjectsService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}