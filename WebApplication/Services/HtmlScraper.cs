using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using WebApplication.Models.enums;

namespace WebApplication.Services;

public class ScrapedTimetableDto
{
    public string TeacherTitle { get; set; } = "mgr";
    public string TeacherFirstName { get; set; } = "N/A";
    public string TeacherLastName { get; set; } = "N/A";
    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string SubjectName { get; set; } = "Brak nazwy";
    public ClassType ClassType { get; set; }
    public string GroupName { get; set; } = "Brak grupy";
    public string RoomNumber { get; set; } = "Brak sali";
    public string BuildingName { get; set; } = "WI";
    public WeekCycle WeekCycle { get; set; }
    public string FieldOfStudyName { get; set; } = "Informatyka";
}

public class HtmlScraper
{
    public List<ScrapedTimetableDto> ParseTimetable(string htmlContent)
    {
        var timetables = new List<ScrapedTimetableDto>();
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        var teacherNode = doc.DocumentNode.SelectSingleNode("//h2[contains(text(), 'Tygodniowy rozkład zajęć nauczyciela')]");
        var teacherRaw = teacherNode?.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()?.Trim();
        
        string tTitle = "mgr", tFirst = "N/A", tLast = "N/A";

        if (!string.IsNullOrEmpty(teacherRaw))
        {
            var parts = WebUtility.HtmlDecode(teacherRaw).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length >= 3)
            {
                tLast = parts.Last();
                tFirst = parts[parts.Length - 2];
                tTitle = string.Join(" ", parts.Take(parts.Length - 2));
            }
            else if (parts.Length == 2)
            {
                tFirst = parts[0];
                tLast = parts[1];
            }
            else if (parts.Length == 1)
            {
                tFirst = parts[0];
                tLast = parts[0]; 
            }
        }

        var daysMap = new Dictionary<string, int> { { "poniedziałek", 1 }, { "wtorek", 2 }, { "środa", 3 }, { "czwartek", 4 }, { "piątek", 5 } };
        var detailHeader = doc.DocumentNode.SelectSingleNode("//h2[text()='Szczegółowy rozkład zajęć']");
        if (detailHeader == null) return timetables;

        var currentNode = detailHeader.NextSibling;
        int currentDayOfWeek = 0;
        var lineRegex = new Regex(@"^(\d{1,2}:\d{2})-(\d{1,2}:\d{2}):\s+(.+?)\s+\((.*?)\)\s+(grupa\s+[^,]+),\s+sala\s+([^,]+)(?:,\s+(.*))?$");

        while (currentNode != null)
        {
            if (currentNode.Name == "h4")
            {
                var dayName = currentNode.InnerText.Trim().ToLower();
                if (daysMap.ContainsKey(dayName)) currentDayOfWeek = daysMap[dayName];
            }
            else if (currentNode.Name == "p" && currentDayOfWeek != 0)
            {
                var lines = currentNode.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var text = WebUtility.HtmlDecode(line).Trim();
                    if (string.IsNullOrEmpty(text) || text.Contains("Konsultacje:")) continue;

                    var match = lineRegex.Match(text);
                    if (match.Success)
                    {
                        var room = match.Groups[6].Value.Trim();
                        var building = room.Length > 0 ? room.Substring(0, 1) : "WI";
                        var extraInfo = match.Groups[7].Value;

                        timetables.Add(new ScrapedTimetableDto
                        {
                            TeacherTitle = string.IsNullOrWhiteSpace(tTitle) ? "mgr" : tTitle,
                            TeacherFirstName = string.IsNullOrWhiteSpace(tFirst) ? "N/A" : tFirst,
                            TeacherLastName = string.IsNullOrWhiteSpace(tLast) ? "N/A" : tLast,
                            DayOfWeek = currentDayOfWeek,
                            StartTime = TimeSpan.Parse(match.Groups[1].Value),
                            EndTime = TimeSpan.Parse(match.Groups[2].Value),
                            SubjectName = match.Groups[3].Value.Trim(),
                            ClassType = MapClassType(match.Groups[4].Value.Trim()),
                            GroupName = match.Groups[5].Value.Trim(),
                            RoomNumber = room,
                            BuildingName = building,
                            WeekCycle = MapWeekCycle(extraInfo),
                            FieldOfStudyName = extraInfo.Contains("kier.") ? Regex.Match(extraInfo, @"kier\.\s+([^,]+)").Groups[1].Value : "Inne"
                        });
                    }
                }
            }
            currentNode = currentNode.NextSibling;
        }
        return timetables;
    }

    private ClassType MapClassType(string abbr) => abbr switch
    {
        "W" => ClassType.Lecture,
        "L" => ClassType.Laboratory,
        "Ć" => ClassType.Exercise,
        "S" => ClassType.Seminar,
        "P" => ClassType.Project,
        "Ps" => ClassType.SpecialisedLaboratory,
        _ => ClassType.Lecture
    };

    private WeekCycle MapWeekCycle(string text)
    {
        if (string.IsNullOrEmpty(text)) return WeekCycle.Weekly;
        if (text.Contains("nieparzyste")) return WeekCycle.Odd;
        if (text.Contains("parzyste")) return WeekCycle.Even;
        return WeekCycle.Weekly;
    }
}