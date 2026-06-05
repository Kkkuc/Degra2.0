using Microsoft.AspNetCore.Mvc;
using WebApplication.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

[Authorize(Roles = "Moderator")]
public class AdminController : Controller
{

    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _context.Users.Include(u => u.Role).ToListAsync();
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> UtworzKonto()
    {
        var roles = await _context.Roles.ToListAsync();
        ViewBag.Roles = new SelectList(roles, "Id", "Name");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UtworzKonto(string username, string email, string tempPassword, int roleId, string? studentId, int? teacherId)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username))
        {
            ModelState.AddModelError(string.Empty, "Użytkownik o takiej nazwie już istnieje.");
            ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Id", "Name");
            return View();
        }

        var newUser = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword, 14), //argon2id
            RoleId = roleId,
            StudentId = studentId,
            TeacherId = teacherId
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Konto dla {username} zostało utworzone!";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> GenerateMonthlyStats(int rok, int miesiac)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        // Domyślne wartości, jeśli admin nic nie wybrał
        if (rok == 0) rok = DateTime.Now.Year;
        if (miesiac == 0) miesiac = DateTime.Now.Month;

        // ODZWIERCIEDLENIE KURSORA PL/SQL W LINQ:
        // Filtrujemy po roku i miesiącu, grupujemy po TableName oraz Operation, zliczamy i sortujemy
        var daneStatystyk = await _context.Logs
            .Where(l => l.ChangedAt.Year == rok && l.ChangedAt.Month == miesiac)
            .GroupBy(l => new { l.TableName, l.Operation })
            .Select(g => new
            {
                TableName = g.Key.TableName,
                Operation = g.Key.Operation,
                OpsCount = g.Count()
            })
            .OrderByDescending(g => g.OpsCount)
            .ThenBy(g => g.TableName)
            .ToListAsync();

        string[] nazwyMiesiecy = { "", "Styczeń", "Luty", "Marzec", "Kwiecień", "Maj", "Czerwiec", "Lipiec", "Sierpień", "Wrzesień", "Październik", "Listopad", "Grudzień" };
        string nazwaMiesiacaTekst = miesiac >= 1 && miesiac <= 12 ? nazwyMiesiecy[miesiac] : miesiac.ToString();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                // NAGŁÓWEK (Identyczny z Twoim DBMS_OUTPUT)
                page.Header().Column(column =>
                {
                    column.Item().Text("=========================================").FontColor(Colors.Grey.Medium);
                    column.Item().Text($"STATYSTYKI AUDYTU ZA: {miesiac:D2}/{rok} ({nazwaMiesiacaTekst.ToUpper()})").Bold().FontSize(14);
                    column.Item().Text("=========================================").FontColor(Colors.Grey.Medium);
                });

                // ZAWARTOŚĆ
                page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                {
                    if (!daneStatystyk.Any())
                    {
                        column.Item().PaddingTop(20).Text("Brak operacji w wybranym miesiącu.").Italic().FontColor(Colors.Grey.Darken1);
                        return;
                    }

                    column.Item().Table(table =>
                    {
                        // Definicja 3 kolumn jak w Twoim komunikacie tekstowym
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Tabela
                            columns.RelativeColumn(2); // Operacja
                            columns.RelativeColumn(1); // Ilość (OpsCount)
                        });

                        // Nagłówki kolumn
                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).Padding(5).Text("Nazwa Tabeli").Bold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Operacja").Bold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Ilość zmian").Bold();
                        });

                        int v_total_operations = 0;

                        // Pętla odpowiadająca: FOR r_stat IN c_stats LOOP
                        foreach (var r_stat in daneStatystyk)
                        {
                            var actionColor = r_stat.Operation == "DELETE" ? Colors.Red.Medium :
                                              r_stat.Operation == "INSERT" ? Colors.Green.Medium : Colors.Blue.Medium;

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(r_stat.TableName);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(r_stat.Operation).FontColor(actionColor).Bold();
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(r_stat.OpsCount.ToString());

                            v_total_operations += r_stat.OpsCount;
                        }

                        // PODSUMOWANIE (Odpowiednik sekcji v_total_operations na końcu procedury)
                        table.Cell().PaddingTop(15).Text("-----------------------------------------").FontColor(Colors.Grey.Medium);
                        table.Cell().Text(""); table.Cell().Text("");

                        table.Cell().Padding(5).Text("SUMA WSZYSTKICH ZMIAN IN BAZIE:").Bold();
                        table.Cell().Padding(5).Text("");
                        table.Cell().Padding(5).Text(v_total_operations.ToString()).Bold();
                    });
                });

                // STOPKA
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Strona "); x.CurrentPageNumber(); x.Span(" z "); x.TotalPages();
                });
            });
        });

        byte[] pdfBytes = document.GeneratePdf();
        return File(pdfBytes, "application/pdf", $"Statystyki_Miesieczne_{rok}_{miesiac:D2}.pdf");
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