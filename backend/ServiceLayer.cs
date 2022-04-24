namespace backend;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

class ServiceLayer
{
    public ServiceLayer(TecAirContext context)
    {
        db = context;
    }

    public IResult CheckLogin(string username, string password)
    {
        var user = (from u in db.Users where u.Username == username select u).SingleOrDefault();
        if (user == null || user.Hash == null || user.Salt == null)
        {
            return Results.Unauthorized();
        }

        var salt = Convert.FromHexString(user.Salt);
        var hash = Convert.FromHexString(user.Hash);
        var challenge = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 1000, 16);

        return hash.SequenceEqual(challenge) ? Results.Ok(user.Id) : Results.Unauthorized();
    }

    public IResult AddUser(NewUser user)
    {
        var salt = new byte[16];
        random.NextBytes(salt);

        var hash = KeyDerivation.Pbkdf2(user.Password, salt, KeyDerivationPrf.HMACSHA256, 1000, 16);
        var row = new User
        {
            Username = user.Username,
            Salt = Convert.ToHexString(salt),
            Hash = Convert.ToHexString(hash),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phonenumber = user.PhoneNumber,
            Email = user.Email,
            University = user.University,
            StudentId = user.StudentId,
            Type = user.Type,
        };

        db.Users.Add(row);
        return save() ?? Results.Ok(row.Id);
    }

    public IResult GetUser(Guid id)
    {
        var user = (from u in db.Users where u.Id == id select u).SingleOrDefault();
        if (user == null)
        {
            return Results.NotFound();
        }

        user.Hash = null;
        user.Salt = null;
        return Results.Ok(user);
    }

    public IResult UpdateUser(Guid id, EditUser edit)
    {
        var user = (from u in db.Users where u.Id == id select u).SingleOrDefault();
        if (user == null)
        {
            return Results.NotFound();
        }

        user.FirstName = edit.FirstName;
        user.LastName = edit.LastName;
        user.Phonenumber = edit.PhoneNumber;
        user.Email = edit.Email;
        user.University = edit.University;
        user.StudentId = edit.StudentId;

        return save() ?? Results.Ok();
    }

    public IResult DeleteUser(Guid id)
    {
        db.Entry(new User { Id = id }).State = EntityState.Deleted;
        return save() ?? Results.Ok();
    }

    public IResult BookFlight(Guid flightId, NewBooking booking)
    {
        var paxId = booking.Pax;
        var promoId = booking.Promo;
        var flight = (from f in db.Flights where f.Id == flightId select f).SingleOrDefault();
        var pax = (from u in db.Users where u.Id == paxId select u).SingleOrDefault();

        if (flight == null || pax == null)
        {
            return Results.NotFound();
        }

        Promo? promo = null;
        if (promoId != null)
        {
            promo = (from p in db.Promos where p.Id == promoId select p).SingleOrDefault();
            if (promo == null)
            {
                return Results.NotFound();
            }
            else if (flight.State != FlightState.Booking || promo.Flight != flightId)
            {
                return Results.BadRequest();
            }
        }

        var total = promo != null ? promo.Price : flight.Price;

        db.Bookings.Add(new Booking { Flight = flightId, Pax = paxId, Promo = promoId });
        return save() ?? Results.Ok(new Booked { Total = total });
    }

    public IResult DumpFlights()
    {
        return Results.Ok(db.Flights.ToArray());
    }

    public IResult DumpPromos()
    {
        return Results.Ok(db.Promos.ToArray());
    }

    public IResult DumpSegments()
    {
        return Results.Ok(db.Segments.ToArray());
    }

    private TecAirContext db;
    private Random random = new Random();

    private IResult? save()
    {
        try
        {
            db.SaveChanges();
            return null;
        }
        catch (DbUpdateException)
        {
            return Results.BadRequest();
        }
    }
}

public class NewUser
{
    [Required]
    public UserType Type { get; set; }
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? University { get; set; }
    public string? StudentId { get; set; }
}

public class EditUser
{
    [Required]
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? University { get; set; }
    public string? StudentId { get; set; }
}

public class NewBooking
{
    [Required]
    public Guid Pax { get; set; }
    public Guid? Promo { get; set; }
}

public class Booked
{
    public decimal Total { get; set; }
}
