using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.DTOs.StudentGroup;
using WebApplication.Models;
using WebApplication.Services;
using WebApplication.Services.Interfaces;

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
    public async Task<IActionResult> Details(int id)
    {
        var studentGroup = await studentGroupsService.GetByStudentIdWithRelationsAsync(id);
        if (studentGroup == null)
        {
            return NotFound();
        }

        return View(studentGroup);
    }

    // GET: StudentGroup/Create
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View();
    }

    // POST: StudentGroup/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentGroupFormDto dto)
    {
        if (ModelState.IsValid)
        {
            await studentGroupsService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropdownsAsync(dto.StudentId, dto.GroupId);
        return View(dto);
    }
    

    // GET: StudentGroup/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var studentGroup = await studentGroupsService.GetByStudentIdWithRelationsAsync(id);
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
        var success = await studentGroupsService.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
    
    private async Task PopulateDropdownsAsync(int? selectedStudentId = null, int? selectedGroupId = null)
    {
        var groups = await studentGroupsService.GetGroupsDropdownAsync();
        var students = await studentGroupsService.GetStudentsLookupAsync();

        ViewData["GroupId"] = new SelectList(groups, "Key", "Value", selectedGroupId);
        
        // Ponieważ GetStudentsLookupAsync zwraca listę rekordów StudentLookupDto(Id, FullName):
        ViewData["StudentId"] = new SelectList(students, "Id", "FullName", selectedStudentId);
    }
}