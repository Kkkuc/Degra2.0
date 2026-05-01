/*using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrawlerController : ControllerBase
    {
        private readonly TimetableCrawler _crawler;

        public CrawlerController(TimetableCrawler crawler)
        {
            _crawler = crawler;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartCrawling()
        {
            await _crawler.CrawlAndSaveAsync();
            return Ok("Crawling finished successfully.");
        }
    }
}*/