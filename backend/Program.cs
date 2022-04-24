using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using backend;
using backend.dal;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                                    builder =>
                                    {
                                        builder.WithOrigins("http://localhost:5000", "http://127.0.0.1:5000", "http://localhost:4200", "http://127.0.0.1:4200")
                                    .AllowAnyHeader()
                                    .WithMethods("POST", "PUT", "GET", "DELETE");
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

app.MapGet("/users", () =>
{
    using (var db = new TecAirContext())
    {
        return new ServiceLayer(db).DumpUsers();
    }
});

app.MapPost("/users", (NewUser user) =>
{
    using (var db = new TecAirContext())
    {
        return new ServiceLayer(db).AddUser(user);
    }
});

app.MapGet("/users/{id}", (Guid id) =>
{
    using (var db = new TecAirContext())
    {
        return new ServiceLayer(db).GetUser(id);
    }
});

app.MapPut("/users/{id}", (Guid id, EditUser edit) =>
{
    using (var db = new TecAirContext())
    {
        return new ServiceLayer(db).UpdateUser(id, edit);
    }
});

app.MapDelete("/users/{id}", (Guid id) =>
{
    using (var db = new TecAirContext())
    {
        return new ServiceLayer(db).DeleteUser(id);
    }
});

app.MapGet("/flights", () =>
{
    using (var db = new TecAirContext())
    {
        return new ServiceLayer(db).DumpFlights();
    }
});

app.MapPost("/flights/{id}/book", (Guid id, NewBooking booking) =>
{
    using (var db = new TecAirContext())
    {
        return new ServiceLayer(db).BookFlight(id, booking);
    }
});

app.MapGet("/promos", () =>
{
    using (var db = new TecAirContext())
    {
        return new ServiceLayer(db).DumpPromos();
    }
});

app.MapGet("/segments", () =>
{
    using (var db = new TecAirContext())
    {
        return new ServiceLayer(db).DumpSegments();
    }
});

app.MapPost("/search", (string fromLoc, string toLoc) =>
{
    using (var db = new TecAirContext())
    {
        return new ServiceLayer(db).SearchFlights(fromLoc, toLoc);
    }
});

app.Run();
