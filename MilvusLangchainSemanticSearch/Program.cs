using System.Diagnostics.CodeAnalysis;
using MilvusLangchainSemanticSearch.Controllers;

namespace MilvusLangchainSemanticSearch;

public class Program
{
    [Experimental("SKEXP0020")]
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddAuthorization()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()!
            .AddControllers();

        builder.Services.AddMilvus(builder.Configuration);
        
        builder.Services.AddSingleton<SemanticSearchService>();
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}