using Microsoft.AspNetCore.Mvc;

namespace MilvusLangchainSemanticSearch
{
    [ApiController]
    [Route("api")]
    public class SemanticSearchController : ControllerBase
    {
        private readonly SemanticSearchService _searchService;

        public SemanticSearchController(SemanticSearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("initialize")]
        public async Task<IActionResult> InitializeDatabase()
        {
            await _searchService.InitializeDatabaseAsync();
            return Ok("Database initialized with Harry Potter document.");
        }

        [HttpGet("ask")]
        public async Task<IActionResult> AskQuestion([FromQuery] string question)
        {
            var answer = await _searchService.GetAnswerAsync(question);
            return Ok(new { Question = question, Answer = answer });
        }
    }
}