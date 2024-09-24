using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel.Connectors.Milvus;
using Microsoft.SemanticKernel.Memory;
using MilvusLangchainSemanticSearch.Milvus;

namespace MilvusLangchainSemanticSearch.Controllers;

public static class MilvusExtensions
{
    [Experimental("SKEXP0020")]
    public static void AddMilvus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMemoryStore, MilvusMemoryStore>();
        services.AddSingleton<MilvusService>(sp =>
        {
            var milvusHost = configuration["Milvus:Host"];
            var milvusPort = int.Parse(configuration["Milvus:Port"]!);
            var openAiApiKey = configuration["OpenAiApiKey"];
            var memoryStore = new MilvusMemoryStore(milvusHost!, milvusPort);

            return new MilvusService(memoryStore, openAiApiKey!);
        });
    }
}