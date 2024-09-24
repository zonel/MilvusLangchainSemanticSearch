using LangChain.Databases;
using LangChain.Databases;
using LangChain.Databases.SemanticKernel;
using LangChain.Providers.OpenAI;
using Microsoft.SemanticKernel.Memory;

namespace MilvusLangchainSemanticSearch.Milvus
{
    public class MilvusService
    {
        private readonly SemanticKernelMemoryDatabase _vectorDatabase;
        private readonly OpenAiEmbeddingModel _embeddingModel;

        public MilvusService(IMemoryStore memoryStore, string openAiApiKey)
        {
            _vectorDatabase = new SemanticKernelMemoryDatabase(memoryStore);

            var openAiProvider = new OpenAiProvider(openAiApiKey);
            _embeddingModel = new OpenAiEmbeddingModel(openAiProvider, id: "text-embedding-ada-002");
        }

        public async Task CreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default)
        {
            await _vectorDatabase.CreateCollectionAsync(collectionName, dimensions, cancellationToken);
        }

        public async Task<IVectorCollection> GetOrCreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default)
        {
            return await _vectorDatabase.GetOrCreateCollectionAsync(collectionName, dimensions, cancellationToken);
        }

        public async Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
        {
            await _vectorDatabase.DeleteCollectionAsync(collectionName, cancellationToken);
        }

        public async Task<bool> IsCollectionExistsAsync(string collectionName, CancellationToken cancellationToken = default)
        {
            return await _vectorDatabase.IsCollectionExistsAsync(collectionName, cancellationToken);
        }

        public async Task<IReadOnlyList<string>> ListCollectionsAsync(CancellationToken cancellationToken = default)
        {
            return await _vectorDatabase.ListCollectionsAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<string>> AddDataWithEmbeddingAsync(string collectionName, string text, CancellationToken cancellationToken = default)
        {
            var embedding = await _embeddingModel.CreateEmbeddingsAsync(text, cancellationToken: cancellationToken);

            var vector = new Vector
            {
                Text = text,
                Embedding = embedding.ToSingleArray(),
                Metadata = new Dictionary<string, object>
                {
                    { "description", "OpenAI generated embedding" },
                    { "source", "OpenAI" }
                }
            };

            var collection = await _vectorDatabase.GetOrCreateCollectionAsync(collectionName, embedding.Dimensions, cancellationToken);

            var insertedIds = await collection.AddAsync(new List<Vector> { vector }, cancellationToken);
            return insertedIds;
        }
    }
}