using Navi.Application.Interfaces;
using Navi.Infrastructure.Services;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Registramos los servicios en memoria
builder.Services.AddSingleton<IChatService, InMemoryChatService>();
builder.Services.AddSingleton<IRagService, InMemoryRagService>();
builder.Services.AddSingleton<ILlmService, InMemoryLlmService>(); 
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
