using LangChain.Databases.Sqlite;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers.Ollama;
using Ollama;

namespace MilvusLangchainSemanticSearch
{
    public class SemanticSearchService
    {
        private readonly OllamaProvider _provider;
        private readonly OllamaEmbeddingModel _embeddingModel;
        private readonly OllamaChatModel _llm;
        private readonly SqLiteVectorDatabase _vectorDatabase;

        public SemanticSearchService()
        {
            _provider = new OllamaProvider("http://localhost:11434/api");
            _embeddingModel = new OllamaEmbeddingModel(_provider, id: "all-minilm");
            _llm = new OllamaChatModel(_provider, id: "llama3");

            // Use SQLite as a vector database
            _vectorDatabase = new SqLiteVectorDatabase(dataSource: "vectors.db");
        }

        public async Task InitializeDatabaseAsync()
        {
            // Load documents from the provided URL and store embeddings
            var vectorCollection = await _vectorDatabase.AddDocumentsFromAsync<PdfPigPdfLoader>(
                _embeddingModel, // Used to convert text to embeddings
                dimensions: 384, // Embedding dimension for all-minilm
                dataSource: DataSource.FromUrl("https://canonburyprimaryschool.co.uk/wp-content/uploads/2016/01/Joanne-K.-Rowling-Harry-Potter-Book-1-Harry-Potter-and-the-Philosophers-Stone-EnglishOnlineClub.com_.pdf"),
                collectionName: "harrypotter",
                textSplitter: null,
                behavior: AddDocumentsToDatabaseBehavior.JustReturnCollectionIfCollectionIsAlreadyExists);
        }

        public async Task<string> GetAnswerAsync(string question)
        {
            // Search for similar documents
            var vectorCollection = await _vectorDatabase.GetCollectionAsync("harrypotter");
            var similarDocuments = await vectorCollection.GetSimilarDocuments(_embeddingModel, question, amount: 5);

            // Use similar documents and LLM to answer the question
            var answer = await _llm.GenerateAsync(
                $"""
                Use the following pieces of context to answer the question at the end.
                If the answer is not in context then just say that you don't know, don't try to make up an answer.
                Keep the answer as short as possible.

                {similarDocuments.AsString()}

                Question: {question}
                Helpful Answer:
                """);

            return answer.ToString()!;
        }
    }
}
