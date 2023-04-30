using BoletoNetCore.WebAPI.SwaggerSetup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwagger();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSwaggerUI();

app.MapControllers();

app.Run();
