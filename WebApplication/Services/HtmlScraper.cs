using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WebApplication.Services
{
    public class ScrapedTimetableDto
    {
        public string TeacherTitle { get; set; }
        public string TeacherFirstName { get; set; }
        public string TeacherLastName { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string SubjectName { get; set; }
        public string ClassTypeAbbr { get; set; }
        public string GroupName { get; set; }
        public string RoomNumber { get; set; }
        public string WeekCycle { get; set; }
        public string ModeDegree { get; set; }
        public string FieldOfStudyName { get; set; }
        public string SemesterNumber { get; set; }
        public string SpecializationName { get; set; }
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

            string tTitle = "";
            string tFirst = "";
            string tLast = "";

            if (!string.IsNullOrEmpty(teacherRaw))
            {
                var match = Regex.Match(teacherRaw, @"^(.*?)\s+([A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]+)\s+([A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]+(-[A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]+)?)$");
                if (match.Success)
                {
                    tTitle = match.Groups[1].Value.Trim();
                    tFirst = match.Groups[2].Value.Trim();
                    tLast = match.Groups[3].Value.Trim();
                }
                else
                {
                    var parts = teacherRaw.Split(' ');
                    if (parts.Length >= 2)
                    {
                        tLast = parts.Last();
                        tFirst = parts[parts.Length - 2];
                        tTitle = string.Join(" ", parts.Take(parts.Length - 2));
                    }
                }
            }

            var daysMap = new Dictionary<string, int>
            {
                { "poniedziałek", 1 }, { "wtorek", 2 }, { "środa", 3 },
                { "czwartek", 4 }, { "piątek", 5 }, { "sobota", 6 }, { "niedziela", 7 }
            };

            var detailHeader = doc.DocumentNode.SelectSingleNode("//h2[text()='Szczegółowy rozkład zajęć']");
            if (detailHeader == null) return timetables;

            var currentNode = detailHeader.NextSibling;
            int currentDayOfWeek = 0;

            var baseRegex = new Regex(@"^(\d{1,2}:\d{2})-(\d{1,2}:\d{2}):\s+(.+?)\s+\((.*?)\)\s+(grupa\s+[^,]+),\s+sala\s+([^,]+),\s+(.+)$");

            while (currentNode != null)
            {
                if (currentNode.Name == "h4")
                {
                    var dayName = currentNode.InnerText.Trim().ToLower();
                    if (daysMap.ContainsKey(dayName))
                    {
                        currentDayOfWeek = daysMap[dayName];
                    }
                }
                else if (currentNode.Name == "p" && currentDayOfWeek != 0)
                {
                    var lines = currentNode.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in lines)
                    {
                        var text = WebUtility.HtmlDecode(line).Trim();

                        if (string.IsNullOrEmpty(text) || text.Contains("Konsultacje:"))
                            continue;

                        var classMatch = baseRegex.Match(text);

                        if (classMatch.Success)
                        {
                            var restParts = classMatch.Groups[7].Value.Split(new[] { ", " }, StringSplitOptions.None);
                            
                            string weekCycle = "Co tydzień";
                            string modeDegree = "Brak";
                            string fieldOfStudy = "Brak";
                            string semesterStr = "1";
                            string specialization = "Brak";

                            int index = 0;
                            if (restParts.Length > index && (restParts[index].Contains("tyg") || restParts[index].Contains("zjazd") || restParts[index].Contains("połowa")))
                            {
                                weekCycle = restParts[index].Trim();
                                index++;
                            }
                            if (restParts.Length > index)
                            {
                                modeDegree = restParts[index].Trim();
                                index++;
                            }
                            if (restParts.Length > index && restParts[index].StartsWith("kier. "))
                            {
                                fieldOfStudy = restParts[index].Substring(6).Trim();
                                index++;
                            }
                            if (restParts.Length > index && restParts[index].StartsWith("sem. "))
                            {
                                semesterStr = restParts[index].Substring(5).Trim();
                                index++;
                            }
                            if (restParts.Length > index)
                            {
                                specialization = string.Join(", ", restParts.Skip(index)).Trim();
                            }

                            timetables.Add(new ScrapedTimetableDto
                            {
                                TeacherTitle = tTitle,
                                TeacherFirstName = tFirst,
                                TeacherLastName = tLast,
                                DayOfWeek = currentDayOfWeek,
                                StartTime = TimeSpan.Parse(classMatch.Groups[1].Value),
                                EndTime = TimeSpan.Parse(classMatch.Groups[2].Value),
                                SubjectName = classMatch.Groups[3].Value.Trim(),
                                ClassTypeAbbr = classMatch.Groups[4].Value.Trim(),
                                GroupName = classMatch.Groups[5].Value.Trim(),
                                RoomNumber = classMatch.Groups[6].Value.Trim(),
                                WeekCycle = weekCycle,
                                ModeDegree = modeDegree,
                                FieldOfStudyName = fieldOfStudy,
                                SemesterNumber = semesterStr,
                                SpecializationName = specialization
                            });
                        }
                    }
                }

                currentNode = currentNode.NextSibling;
            }

            return timetables;
        }
    }
}