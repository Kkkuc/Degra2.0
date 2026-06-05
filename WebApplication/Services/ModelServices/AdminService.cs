using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WebApplication.Data;
using WebApplication.DTOs.Admin;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class AdminService(AppDbContext context) : IAdminService
{
    public async Task<IEnumerable<UserListDto>> GetUsersForIndexAsync()
    {
        return await context.Users
            .Select(u => new UserListDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                RoleName = u.Role != null ? u.Role.Name : "Brak roli"
            })
            .ToListAsync();
    }

    public async Task<Dictionary<int, string>> GetRolesDropdownListAsync()
    {
        return await context.Roles
            .ToDictionaryAsync(r => r.Id, r => r.Name);
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        return await context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task CreateAccountAsync(CreateAccountDto dto)
    {
        var newUser = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.TempPassword),
            RoleId = dto.RoleId
        };

        context.Users.Add(newUser);
        await context.SaveChangesAsync();
    }

    public async Task<byte[]> GenerateMonthlyStatsPdfAsync(int rok, int miesiac)
    {
        // Domyślne wartości
        rok = rok == 0 ? DateTime.Now.Year : rok;
        miesiac = miesiac == 0 ? DateTime.Now.Month : miesiac;

        var daneStatystyk = await context.Logs
            .Where(l => l.ChangedAt.Year == rok && l.ChangedAt.Month == miesiac)
            .GroupBy(l => new { l.TableName, l.Operation })
            .Select(g => new { g.Key.TableName, g.Key.Operation, OpsCount = g.Count() })
            .OrderByDescending(g => g.OpsCount)
            .ThenBy(g => g.TableName)
            .ToListAsync();

        string[] nazwyMiesiecy =
        {
            "", "Styczeń", "Luty", "Marzec", "Kwiecień", "Maj", "Czerwiec", "Lipiec", "Sierpień", "Wrzesień",
            "Październik", "Listopad", "Grudzień"
        };
        string nazwaMiesiacaTekst = miesiac >= 1 && miesiac <= 12 ? nazwyMiesiecy[miesiac] : miesiac.ToString();
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                // NAGŁÓWEK
                page.Header().Column(column =>
                {
                    column.Item().Text("=========================================").FontColor(Colors.Grey.Medium);
                    column.Item().Text($"STATYSTYKI AUDYTU ZA: {miesiac:D2}/{rok} ({nazwaMiesiacaTekst.ToUpper()})")
                        .Bold().FontSize(14);
                    column.Item().Text("=========================================").FontColor(Colors.Grey.Medium);
                });

                // ZAWARTOŚĆ
                page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                {
                    if (!daneStatystyk.Any())
                    {
                        column.Item().PaddingTop(20).Text("Brak operacji w wybranym miesiącu.").Italic()
                            .FontColor(Colors.Grey.Darken1);
                        return;
                    }

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Tabela
                            columns.RelativeColumn(2); // Operacja
                            columns.RelativeColumn(); // Ilość
                        });

                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).Padding(5).Text("Nazwa Tabeli").Bold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Operacja").Bold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Ilość zmian").Bold();
                        });

                        var vTotalOperations = 0;

                        foreach (var rStat in daneStatystyk)
                        {
                            var actionColor = rStat.Operation == "DELETE" ? Colors.Red.Medium :
                                rStat.Operation == "INSERT" ? Colors.Green.Medium : Colors.Blue.Medium;

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                .Text(rStat.TableName);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                .Text(rStat.Operation).FontColor(actionColor).Bold();
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                .Text(rStat.OpsCount.ToString());

                            vTotalOperations += rStat.OpsCount;
                        }

                        // PODSUMOWANIE
                        table.Cell().PaddingTop(15).Text("-----------------------------------------")
                            .FontColor(Colors.Grey.Medium);
                        table.Cell().Text("");
                        table.Cell().Text("");

                        table.Cell().Padding(5).Text("SUMA WSZYSTKICH ZMIAN W BAZIE:").Bold();
                        table.Cell().Padding(5).Text("");
                        table.Cell().Padding(5).Text(vTotalOperations.ToString()).Bold();
                    });
                });

                // STOPKA
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Strona ");
                    x.CurrentPageNumber();
                    x.Span(" z ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }
}