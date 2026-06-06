using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.DTOs.Admin;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[Authorize(Roles = "Moderator")]
public class AdminController(IAdminService adminService, ITimetablesService timetablesService) : Controller
{
    private async Task LoadViewDataAsync(object? selectedRoleId = null)
    {
        var roles = await adminService.GetRolesDropdownListAsync();
        ViewBag.Roles = new SelectList(roles, "Key", "Value", selectedRoleId);
        
        ViewBag.SubjectId = new SelectList(await timetablesService.GetAllSubjectsAsync(), "Id", "Name");
        ViewBag.TeacherId = new SelectList(await timetablesService.GetAllTeachersAsync(), "Id", "FirstName");
        ViewBag.RoomId = new SelectList(await timetablesService.GetAllRoomsAsync(), "Id", "RoomNumber");
        ViewBag.GroupId = new SelectList(await timetablesService.GetAllGroupsAsync(), "Id", "Name");
    }
    
    
    public async Task<IActionResult> Index()
    {
        var data = await adminService.GetUsersForIndexAsync();
        await LoadViewDataAsync();
        return View(data);
    }

    [HttpGet]
    public IActionResult CreateAccount() => RedirectToAction(nameof(Index));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAccount(CreateAccountDto dto)
    {
        if (!ModelState.IsValid || await adminService.UserExistsAsync(dto.Username))
        {
            if (await adminService.UserExistsAsync(dto.Username))
            {
                ModelState.AddModelError(string.Empty, "Użytkownik o takiej nazwie już istnieje.");
            }
            
            // Dzięki wspólnej metodzie, nie musisz kopiować ViewBag-ów
            await LoadViewDataAsync(dto.RoleId);
            
            var data = await adminService.GetUsersForIndexAsync();
            return View(nameof(Index), data);
        }

        await adminService.CreateAccountAsync(dto);
        TempData["SuccessMessage"] = $"Konto dla {dto.Username} zostało utworzone!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GenerateMonthlyStats(int rok, int miesiac)
    {
        try
        {
            var pdfBytes = await adminService.GenerateMonthlyStatsPdfAsync(rok, miesiac);
            return File(pdfBytes, "application/pdf", $"Statystyki_Miesieczne_{rok}_{miesiac:D2}.pdf");
        }
        catch (Exception)
        {
            // Obsługa błędów, np. logowanie i powrót do widoku z komunikatem
            TempData["ErrorMessage"] = "Wystąpił błąd podczas generowania raportu.";
            return RedirectToAction(nameof(Index));
        }
    }

    /*
    [HttpGet]
    public async Task<IActionResult> GenerujRaportLogowPdf()
    {
        // Wymagane przez twórców biblioteki QuestPDF dla darmowych projektów
        QuestPDF.Settings.License = LicenseType.Community;

        // Pobieramy 100 najnowszych logów z bazy Oracle
        var logs = await _context.Logs
            .OrderByDescending(l => l.ChangedAt)
            .Take(100)
            .ToListAsync();

        // Generowanie dokumentu
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                // Format A4, układ poziomy dla lepszej czytelności długich tekstów
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                // NAGŁÓWEK
                page.Header()
                    .Text("Raport Szlaku Audytowego (Logi Bazy Danych)")
                    .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken2);

                // TREŚĆ (Tabela z logami)
                page.Content().PaddingVertical(1, Unit.Centimetre).Column(x =>
                {
                    x.Item().PaddingBottom(10).Text($"Wygenerowano: {DateTime.Now:yyyy-MM-dd HH:mm} | Ostatnie 100 operacji");

                    x.Item().Table(table =>
                    {
                        // Definiowanie szerokości kolumn
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.5f); // Data
                            columns.RelativeColumn(1.5f); // Tabela
                            columns.RelativeColumn(1);    // Operacja
                            columns.RelativeColumn(1.5f); // Kto zmienił
                            columns.RelativeColumn(3);    // Stara wartość  
                            columns.RelativeColumn(3);    // Nowa wartość
                        });

                        // Nagłówki tabeli
                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).Padding(5).Text("Data").Bold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Tabela").Bold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Akcja").Bold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Użytkownik").Bold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Stare dane").Bold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Nowe dane").Bold();
                        });

                        // Wypełnianie tabeli wierszami
                        foreach (var log in logs)
                        {
                            // Kolorowanie tekstu w zależności od operacji
                            var actionColor = log.Operation.ToUpper() == "DELETE" ? Colors.Red.Medium :
                                              log.Operation.ToUpper() == "INSERT" ? Colors.Green.Medium : Colors.Orange.Medium;

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(log.ChangedAt.ToString("yyyy-MM-dd HH:mm"));
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(log.TableName);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(log.Operation).FontColor(actionColor).Bold();
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(log.UserChanged);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(log.OldValue ?? "-").FontColor(Colors.Grey.Darken1);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(log.NewValue ?? "-").FontColor(Colors.Grey.Darken3);
                        }
                    });
                });

                // STOPKA (Numeracja stron)
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Strona ");
                    x.CurrentPageNumber();
                    x.Span(" z ");
                    x.TotalPages();
                });
            });
        });

        // Zapisanie do tablicy bajtów i zwrócenie jako plik
        byte[] pdfBytes = document.GeneratePdf();
        return File(pdfBytes, "application/pdf", $"Raport_Audytu_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
    }
    */
}