/* Punto de entrada de la aplicación.
 */

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using backend;
using backend.dal;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<ServiceLayer>();
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

/*
 * Endpoints, ver ServiceLayer.cs para descripción de cada uno.
 */

app.MapPost("/check_login", (string username, string password) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).CheckLogin(username, password);
    }
});

app.MapGet("/users", () =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).DumpUsers();
    }
});

app.MapPost("/users", (NewUser user) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).AddUser(user);
    }
});

app.MapGet("/users/{id}", (Guid id) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).GetUser(id);
    }
});

app.MapPut("/users/{id}", (Guid id, EditUser edit) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).UpdateUser(id, edit);
    }
});

app.MapDelete("/users/{id}", (Guid id) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).DeleteUser(id);
    }
});

app.MapGet("/users/{id}/open", (Guid id) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).GetOpenFlights(id);
    }
});

app.MapGet("/users/{id}/checked", (Guid id) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).GetCheckedFlights(id);
    }
});

app.MapGet("/flights", () =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).DumpFlights(false);
    }
});

app.MapGet("/flights/booking", () =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).DumpFlights(true);
    }
});

app.MapPost("/flights", (NewFlight flight) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).AddFlight(flight);
    }
});

app.MapGet("/flights/{id}", (Guid id) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).GetFlight(id);
    }
});

app.MapDelete("/flights/{id}", (Guid id) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).DeleteFlight(id);
    }
});

app.MapPost("/flights/{id}/book", (Guid id, NewBooking booking) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).BookFlight(id, booking);
    }
});

app.MapPost("/flights/{id}/bag", (Guid id, NewBag bag) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).AddBag(id, bag);
    }
});

app.MapPost("/flights/{id}/open", (Guid id) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).OpenFlight(id);
    }
});

app.MapPost("/flights/{id}/close", (Guid id) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).CloseFlight(id);
    }
});

app.MapPost("/flights/{id}/reset", (Guid id) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).ResetFlight(id);
    }
});

app.MapGet("/promos", () =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).DumpPromos();
    }
});

app.MapGet("/promos/search", (string code) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).SearchPromo(code);
    }
});

app.MapDelete("/promos/{id}", (Guid id) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).DeletePromo(id);
    }
});

app.MapGet("/segments", () =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).DumpSegments(false);
    }
});

app.MapGet("/segments/booking", () =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).DumpSegments(true);
    }
});

app.MapPost("/segments/{id}/checkin", (Guid id, CheckIn checkIn) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).CheckIn(id, checkIn);
    }
});

app.MapPost("/search", (string fromLoc, string toLoc) =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).SearchFlights(fromLoc, toLoc);
    }
});

app.MapGet("/airports", () =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).DumpAirports();
    }
});

app.MapGet("/aircraft", () =>
{
    using (var db = new TecAirContext(app))
    {
        return new ServiceLayer(db).DumpAircraft();
    }
});

app.Run();
