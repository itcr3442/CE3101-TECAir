namespace backend;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Npgsql;
using backend.dal;

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
        var challenge = hashFor(password, salt);

        return hash.SequenceEqual(challenge) ? Results.Ok(user.Id) : Results.Unauthorized();
    }

    public IResult AddUser(NewUser user)
    {
        var salt = randomSalt();
        var hash = hashFor(user.Password, salt);

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

        var newSalt = randomSalt();

        user.Username = edit.Username;
        user.Salt = Convert.ToHexString(newSalt);
        user.Hash = Convert.ToHexString(hashFor(edit.Password, newSalt));
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
        db.Bookings.RemoveRange(from b in db.Bookings where b.Pax == id select b);
        db.Bags.RemoveRange(from b in db.Bags where b.Owner == id select b);
        db.Checkins.RemoveRange(from c in db.Checkins where c.Pax == id select c);
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
        else if (flight.State != FlightState.Booking)
        {
            return Results.BadRequest();
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

    public IResult OpenFlight(Guid flightId)
    {
        var flight = (from f in db.Flights where f.Id == flightId select f).SingleOrDefault();
        if (flight == null)
        {
            return Results.NotFound();
        }

        switch (flight.State)
        {
            // Ya está abierto
            case FlightState.Checkin:
                return Results.Ok();

            case FlightState.Booking:
                flight.State = FlightState.Checkin;
                return save() ?? Results.Ok();

            default:
                return Results.BadRequest();
        }
    }

    public IResult CloseFlight(Guid flightId)
    {
        var flight = (from f in db.Flights where f.Id == flightId select f).SingleOrDefault();
        if (flight == null)
        {
            return Results.NotFound();
        }

        switch (flight.State)
        {
            // Ya está cerrado
            case FlightState.Closed:
                return Results.Ok();

            case FlightState.Checkin:
                flight.State = FlightState.Closed;
                return save() ?? Results.Ok();

            default:
                return Results.BadRequest();
        }
    }

    public IResult DumpUsers()
    {
        var users = db.Users.ToArray();
        foreach (var user in users)
        {
            user.Hash = null;
            user.Salt = null;
        }

        return Results.Ok(users);
    }

    public IResult DumpFlights(bool filterBooking)
    {
        IEnumerable<Flight> flights = db.Flights;
        if (filterBooking)
        {
            flights = flights.Where(f => f.State == FlightState.Booking);
        }

        return Results.Ok(flights.ToArray());
    }

    public IResult DumpPromos()
    {
        return Results.Ok(db.Promos.ToArray());
    }

    public IResult DumpSegments(bool filterBooking)
    {
        var segments = db.Segments.ToList();
        if (filterBooking)
        {
            segments = segments.Where(s => s.FlightNavigation.State == FlightState.Booking).ToList();
        }

        var tagged = from segment in segments
                     select new TaggedSegment
                     {
                         Segment = segment,
                         From = segment.FromLocNavigation,
                         To = segment.ToLocNavigation
                     };

        return Results.Ok(tagged.ToArray());
    }

    public IResult SearchFlights(string fromLoc, string toLoc)
    {
        var fromPort = (from a in db.Airports where a.Code == fromLoc select a).SingleOrDefault();
        var toPort = (from a in db.Airports where a.Code == toLoc select a).SingleOrDefault();

        if (fromPort == null || toPort == null)
        {
            return Results.Ok(new SearchResult[] { });
        }

        var fromSegments = from s in db.Segments where s.FromLoc == fromPort.Id select s;
        var toSegments = from s in db.Segments where s.ToLoc == toPort.Id select s;

        var flights = from fromSeg in fromSegments
                      join toSeg in toSegments
                      on fromSeg.Flight equals toSeg.Flight
                      where fromSeg.SeqNo <= toSeg.SeqNo
                      select fromSeg.FlightNavigation;

        var results = from flight in flights
                      where flight.State == FlightState.Booking
                      select new SearchResult
                      {
                          Flight = flight,
                          From = flight.Endpoint.FromLocNavigation,
                          To = flight.Endpoint.ToLocNavigation
                      };

        return Results.Ok(results.ToArray());
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
        catch (DbUpdateException ex)
        {
            var inner = ex.InnerException as PostgresException;
            if (inner == null)
            {
                throw ex;
            }

            switch (inner.SqlState)
            {
                case PostgresErrorCodes.IntegrityConstraintViolation:
                case PostgresErrorCodes.RestrictViolation:
                case PostgresErrorCodes.NotNullViolation:
                case PostgresErrorCodes.ForeignKeyViolation:
                case PostgresErrorCodes.UniqueViolation:
                case PostgresErrorCodes.CheckViolation:
                case PostgresErrorCodes.ExclusionViolation:
                    return Results.Conflict();

                default:
                    return Results.BadRequest();
            }
        }
    }

    private byte[] randomSalt()
    {
        var salt = new byte[16];
        random.NextBytes(salt);
        return salt;
    }

    private byte[] hashFor(string password, byte[] salt)
    {
        return KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 1000, 16);
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

public class SearchResult
{
    public Flight Flight { get; set; } = null!;
    public Airport From { get; set; } = null!;
    public Airport To { get; set; } = null!;
}

public class TaggedSegment
{
    public Segment Segment { get; set; } = null!;
    public Airport From { get; set; } = null!;
    public Airport To { get; set; } = null!;
}
