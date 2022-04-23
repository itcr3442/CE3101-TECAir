using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using backend;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                                    builder =>
                                    {
                                        builder.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
                                    .AllowAnyHeader()
                                    .WithMethods("POST", "GET", "DELETE");
                                    });
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.MapPost("/check_login", (string username, string password) =>
{
    using (var db = new TecAirContext())
    {
        return new ServiceLayer(db).CheckLogin(username, password);
    }
});

app.Run();
