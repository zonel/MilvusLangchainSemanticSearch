using LangChain.Databases.Sqlite;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Providers.Ollama;
using Microsoft.AspNetCore.Mvc;

namespace MilvusLangchainSemanticSearch
{
    [ApiController]
    [Route("api")]
    public class SemanticSearchController(SemanticSearchService searchService) : ControllerBase
    {
        [HttpGet("initialize")]
        public async Task<IActionResult> InitializeDatabase()
        {
            await searchService.InitializeDatabaseAsync();
            return Ok("Database initialized with Harry Potter document.");
        }

        [HttpGet("ask")]
        public async Task<IActionResult> AskQuestion([FromQuery] string question)
        {
            var answer = await searchService.GetAnswerAsync(question);
            return Ok(new { Question = question, Answer = answer });
        }
    }
}