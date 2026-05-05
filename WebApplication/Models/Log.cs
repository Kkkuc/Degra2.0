namespace WebApplication.Models;

public class Log
{
    public int Id { get; set; }
    public string TableName { get; set; }    // Nazwa tabeli (np. 'Groups')
    public string Operation { get; set; }    // INSERT, UPDATE, DELETE
    public string OldValue { get; set; }     // Dane przed zmianą (JSON)
    public string NewValue { get; set; }     // Dane po zmianie (JSON)
    public string UserChanged { get; set; }  // Użytkownik bazy lub aplikacji
    public DateTime ChangedAt { get; set; }  // Data i godzina
}
