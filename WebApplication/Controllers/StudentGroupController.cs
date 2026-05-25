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
        ViewData["StudentId"] = new SelectList(students, "Id", "FullName"); // OK
        return View();
    }

    // POST: StudentGroup/Create
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
        ViewData["StudentId"] = new SelectList(students, "Id", "FullName", studentGroup.StudentId); // POPRAWKA: "FullName" zamiast "FirstName"
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