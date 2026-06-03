using System.Threading.Tasks;

namespace WebApplication.Services.Interfaces;

public interface IScraperService
{
    Task ScrapeAndSaveAsync(string url);
}