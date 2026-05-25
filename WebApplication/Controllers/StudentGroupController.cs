using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers;

public class StudentGroupController(IStudentGroupsService studentGroupsService) : Controller
{
    // GET: StudentGroup
    public async Task<IActionResult> Index()
    {
        var data = await studentGroupsService.GetAllWithRelationsAsync();
        return View(data);
    }

    // GET: StudentGroup/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var studentGroup = await studentGroupsService.GetByStudentIdWithRelationsAsync(id.Value);
        if (studentGroup == null)
        {
            return NotFound();
        }

        return View(studentGroup);
    }

    // GET: StudentGroup/Create
    public async Task<IActionResult> Create()
    {
        var groups = await studentGroupsService.GetAllGroupsAsync();
        var students = await studentGroupsService.GetStudentsLookupAsync();
        ViewData["GroupId"] = new SelectList(groups, "Id", "Name");
        ViewData["StudentId"] = new SelectList(students, "Id", "FullName");
        return View();
    }

    // POST: StudentGroup/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("StudentId,GroupId")] StudentGroup studentGroup)
    {
        if (ModelState.IsValid)
        {
            await studentGroupsService.CreateAsync(studentGroup);
            return RedirectToAction(nameof(Index));
        }
        var groups = await studentGroupsService.GetAllGroupsAsync();
        var students = await studentGroupsService.GetStudentsLookupAsync();
        ViewData["GroupId"] = new SelectList(groups, "Id", "Name", studentGroup.GroupId);
        ViewData["StudentId"] = new SelectList(students, "Id", "FirstName", studentGroup.StudentId);
        return View(studentGroup);
    }

    // GET: StudentGroup/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var studentGroup = await studentGroupsService.GetByStudentIdAsync(id.Value);
        if (studentGroup == null)
        {
            return NotFound();
        }
        var groups = await studentGroupsService.GetAllGroupsAsync();
        var students = await studentGroupsService.GetStudentsLookupAsync();
        ViewData["GroupId"] = new SelectList(groups, "Id", "Name", studentGroup.GroupId);
        ViewData["StudentId"] = new SelectList(students, "Id", "FirstName", studentGroup.StudentId);
        return View(studentGroup);
    }

    // POST: StudentGroup/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("StudentId,GroupId")] StudentGroup studentGroup)
    {
        if (id != studentGroup.StudentId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await studentGroupsService.UpdateAsync(studentGroup);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await studentGroupsService.ExistsAsync(studentGroup.StudentId))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        var groups = await studentGroupsService.GetAllGroupsAsync();
        var students = await studentGroupsService.GetStudentsLookupAsync();
        ViewData["GroupId"] = new SelectList(groups, "Id", "Name", studentGroup.GroupId);
        ViewData["StudentId"] = new SelectList(students, "Id", "FirstName", studentGroup.StudentId);
        return View(studentGroup);
    }

    // GET: StudentGroup/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var studentGroup = await studentGroupsService.GetByStudentIdWithRelationsAsync(id.Value);
        if (studentGroup == null)
        {
            return NotFound();
        }

        return View(studentGroup);
    }

    // POST: StudentGroup/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await studentGroupsService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}