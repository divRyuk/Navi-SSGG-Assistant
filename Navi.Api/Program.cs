using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Navi.Application.Interfaces;
using Navi.Infrastructure.Configurations;
using Navi.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de serialización global de GUIDs para MongoDB
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

// Configuración de MongoDB desde appsettings.json
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Controllers y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Procesador de PDFs
builder.Services.AddSingleton<IDocumentProcessorService, PdfDocumentProcessorService>();

// 2. Servicios HTTP reales de Ollama (Embeddings y LLM)

// 2. Servicios HTTP reales de Ollama (Embeddings y LLM) con Timeout ampliado
builder.Services.AddHttpClient<IEmbeddingService, OllamaEmbeddingService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);
});

builder.Services.AddHttpClient<ILlmService, OllamaLlmService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(8);
});

// 3. Base de Datos MongoDB y Servicio RAG Real
builder.Services.AddScoped<IChatService, MongoChatService>();
builder.Services.AddScoped<IRagService, RagService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();