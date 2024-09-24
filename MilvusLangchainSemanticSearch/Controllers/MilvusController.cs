using Microsoft.AspNetCore.Mvc;

namespace MilvusLangchainSemanticSearch.Milvus
{
    [ApiController]
    [Route("api/[controller]")]
    public class MilvusController : ControllerBase
    {
        private readonly MilvusService _milvusService;

        public MilvusController(MilvusService milvusService)
        {
            _milvusService = milvusService;
        }

        [HttpPost("collections")]
        public async Task<IActionResult> CreateCollection(string collectionName, int dimensions, CancellationToken cancellationToken)
        {
            await _milvusService.CreateCollectionAsync(collectionName, dimensions, cancellationToken);
            return Ok(new { message = $"Collection '{collectionName}' created with dimensions {dimensions}." });
        }

        [HttpPost("collections/{collectionName}/vectors")]
        public async Task<IActionResult> AddDataWithEmbedding(string collectionName, [FromBody] AddVectorRequest request, CancellationToken cancellationToken)
        {
            var insertedIds = await _milvusService.AddDataWithEmbeddingAsync(collectionName, request.Text, cancellationToken);
            return Ok(new { insertedIds });
        }

        [HttpGet("collections")]
        public async Task<IActionResult> ListCollections(CancellationToken cancellationToken)
        {
            var collections = await _milvusService.ListCollectionsAsync(cancellationToken);
            return Ok(collections);
        }

        [HttpGet("collections/{collectionName}/exists")]
        public async Task<IActionResult> CheckCollectionExists(string collectionName, CancellationToken cancellationToken)
        {
            var exists = await _milvusService.IsCollectionExistsAsync(collectionName, cancellationToken);
            return Ok(new { collectionName, exists });
        }

        [HttpDelete("collections/{collectionName}")]
        public async Task<IActionResult> DeleteCollection(string collectionName, CancellationToken cancellationToken)
        {
            await _milvusService.DeleteCollectionAsync(collectionName, cancellationToken);
            return Ok(new { message = $"Collection '{collectionName}' deleted." });
        }
    }

    public class AddVectorRequest
    {
        public string Text { get; set; }
    }
}
