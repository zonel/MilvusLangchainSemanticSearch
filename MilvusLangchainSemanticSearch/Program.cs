namespace MilvusLangchainSemanticSearch;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddAuthorization()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()!
            .AddControllers();

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