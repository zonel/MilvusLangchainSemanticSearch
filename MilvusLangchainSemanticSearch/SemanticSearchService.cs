using LangChain.Databases.Sqlite;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers.OpenAI;
using Ollama;

namespace MilvusLangchainSemanticSearch
{
    public class SemanticSearchService
    {
        private readonly OpenAiProvider _provider;
        private readonly OpenAiEmbeddingModel _embeddingModel;
        private readonly OpenAiChatModel _llm;
        private readonly SqLiteVectorDatabase _vectorDatabase;

        public SemanticSearchService(IConfiguration configuration)
        {
            var OpenAiApiKey = configuration["OpenAiApi:ApiKey"];
            
            _provider = new OpenAiProvider(OpenAiApiKey);
            _embeddingModel = new OpenAiEmbeddingModel(_provider, id: "text-embedding-ada-002");
            _llm = new OpenAiChatModel(_provider, id: "gpt-4o-mini-2024-07-18");
            
            _vectorDatabase = new SqLiteVectorDatabase(dataSource: "vectors.db");
        }

        public async Task InitializeDatabaseAsync()
        {
            var vectorCollection = await _vectorDatabase.AddDocumentsFromAsync<PdfPigPdfLoader>(
                _embeddingModel, 
                dimensions: 384, 
                dataSource: DataSource.FromUrl("https://canonburyprimaryschool.co.uk/wp-content/uploads/2016/01/Joanne-K.-Rowling-Harry-Potter-Book-1-Harry-Potter-and-the-Philosophers-Stone-EnglishOnlineClub.com_.pdf"),
                collectionName: "harrypotter",
                textSplitter: null,
                behavior: AddDocumentsToDatabaseBehavior.JustReturnCollectionIfCollectionIsAlreadyExists);
        }

        public async Task<string> GetAnswerAsync(string question)
        {
            var vectorCollection = await _vectorDatabase.GetCollectionAsync("harrypotter");
            var similarDocuments = await vectorCollection.GetSimilarDocuments(_embeddingModel, question, amount: 5);

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
