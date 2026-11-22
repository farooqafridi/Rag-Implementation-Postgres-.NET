using Application.Features.Documents.GetDocument;
using Application.Interfaces;
using Infrastructure.AI;
using Infrastructure.Documents;
using Infrastructure.Documents.Storage;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//builder.Services.AddDbContext<ApplicationDbContext>(
//    options =>
//        options.UseNpgsql(
//            builder.Configuration.GetConnectionString("DefaultConnection"),
//            o => o.UseVector()
//        )
//);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseVector());

    options.LogTo(Console.WriteLine, LogLevel.Information);

    options.EnableDetailedErrors();

    options.EnableSensitiveDataLogging();
});

builder.Services.AddScoped<DocumentIngestionService1>();
builder.Services.AddScoped<RagService>();
builder.Services.AddScoped<IVectorSearchService, VectorSearchService>();

builder.Services.AddScoped<IDocumentTextExtractor, DocumentTextExtractor>();
builder.Services.AddSingleton<TextChunker>();

// Ollama embedding and chat services
builder.Services.AddHttpClient<IEmbeddingService, OllamaEmbeddingService>(
    client =>
    {
        client.BaseAddress = new Uri("http://localhost:11434");
    });

builder.Services.AddHttpClient<IChatService, OllamaChatService>(
    client =>
    {
        client.BaseAddress = new Uri("http://localhost:11434");
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDocumentStorage, LocalDocumentStorage>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
//builder.Services.AddScoped<DocumentIngestionService2>();
builder.Services.AddScoped<IDocumentIngestionService, DocumentIngestionService>();
builder.Services.AddScoped<GetDocumentHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
