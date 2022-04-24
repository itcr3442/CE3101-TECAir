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
        var challenge = HashFor(password, salt);

        return hash.SequenceEqual(challenge) ? Results.Ok(user.Id) : Results.Unauthorized();
    }

    public IResult AddUser(NewUser user)
    {
        var salt = RandomSalt();
        var hash = HashFor(user.Password, salt);

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
        return Save() ?? Results.Ok(row.Id);
    }

    public IResult GetUser(Guid id)
    {
        var user = (from u in db.Users where u.Id == id select u).SingleOrDefault();
        if (user == null)
        {
            return Results.NotFound();
        }

        var plain = new User
        {
            Id = user.Id,
            Username = user.Username,
            Hash = null,
            Salt = null,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phonenumber = user.Phonenumber,
            Email = user.Email,
            University = user.University,
            StudentId = user.StudentId,
            Type = user.Type,
        };

        return Results.Ok(user);
    }

    public IResult UpdateUser(Guid id, EditUser edit)
    {
        var user = (from u in db.Users where u.Id == id select u).SingleOrDefault();
        if (user == null)
        {
            return Results.NotFound();
        }


        user.Username = edit.Username ?? user.Username;
        if (edit.Password != null)
        {
            var newSalt = RandomSalt();
            user.Salt = Convert.ToHexString(newSalt);
            user.Hash = Convert.ToHexString(HashFor(edit.Password, newSalt));
        }
        user.FirstName = edit.FirstName ?? user.FirstName;
        user.LastName = edit.LastName ?? user.LastName;
        user.Phonenumber = edit.PhoneNumber ?? user.Phonenumber;
        user.Email = edit.Email ?? user.Email;
        user.University = edit.University ?? user.University;
        user.StudentId = edit.StudentId ?? user.StudentId;

        return Save() ?? Results.Ok();
    }

    public IResult DeleteUser(Guid id)
    {
        db.Bookings.RemoveRange(from b in db.Bookings where b.Pax == id select b);
        db.Bags.RemoveRange(from b in db.Bags where b.Owner == id select b);
        db.Checkins.RemoveRange(from c in db.Checkins where c.Pax == id select c);
        db.Entry(new User { Id = id }).State = EntityState.Deleted;

        return Save() ?? Results.Ok();
    }

    public IResult GetFlight(Guid flightId)
    {
        var flight = db.Flights.Where(f => f.Id == flightId).SingleOrDefault();
        if (flight == null)
        {
            return Results.NotFound();
        }

        var plain = new Flight
        {
            Id = flight.Id,
            No = flight.No,
            State = flight.State,
            Comment = flight.Comment,
            Price = flight.Price,
        };

        return flight != null ? Results.Ok(plain) : Results.NotFound();
    }

    public IResult DeleteFlight(Guid flightId)
    {
        var flight = db.Flights.Where(f => f.Id == flightId).SingleOrDefault();
        if (flight == null)
        {
            return Results.NotFound();
        }

        db.Bookings.RemoveRange(db.Bookings.Where(b => b.Flight == flightId));
        db.Bags.RemoveRange(db.Bags.Where(b => b.Flight == flightId));
        db.Promos.RemoveRange(db.Promos.Where(p => p.Flight == flightId));

        foreach (var segment in db.Segments.Where(s => s.Flight == flightId).ToArray())
        {
            db.Checkins.RemoveRange(db.Checkins.Where(c => c.Segment == segment.Id));
            db.Segments.Remove(segment);
        }

        db.Flights.Remove(flight);
        return Save() ?? Results.Ok();
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
        return Save() ?? Results.Ok(new Booked { Total = total });
    }

    public IResult AddBag(Guid flightId, NewBag bag)
    {
        var validColor = bag.Color.Length == 7 && bag.Color[0] == '#';
        if (validColor)
        {
            try
            {
                Convert.FromHexString(bag.Color.Substring(1));
            }
            catch (FormatException)
            {
                validColor = false;
            }
        }

        if (bag.Weight < 0 || bag.Weight > 1000 || !validColor)
        {
            return Results.BadRequest();
        }

        var flight = (from f in db.Flights where f.Id == flightId select f).SingleOrDefault();
        var pax = (from u in db.Users where u.Id == bag.Owner select u).SingleOrDefault();

        if (flight == null || pax == null)
        {
            return Results.NotFound();
        }

        var checkins = from checkin in db.Checkins
                       join segment in db.Segments
                       on checkin.Segment equals segment.Id
                       where checkin.Pax == bag.Owner && segment.Flight == flightId
                       select 1;

        if (checkins.Count() == 0)
        {
            return Results.BadRequest();
        }

        var row = new Bag
        {
            Owner = bag.Owner,
            Flight = flightId,
            Weight = bag.Weight,
            Color = bag.Color,
        };

        db.Bags.Add(row);
        return Save() ?? Results.Ok(new InsertedBag { Id = row.Id, No = row.No });
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
                return Save() ?? Results.Ok();

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
                return Save() ?? Results.Ok();

            default:
                return Results.BadRequest();
        }
    }

    public IResult ResetFlight(Guid flightId)
    {
        var flight = (from f in db.Flights where f.Id == flightId select f).SingleOrDefault();
        if (flight == null)
        {
            return Results.NotFound();
        }

        db.Bookings.RemoveRange(from b in db.Bookings where b.Flight == flightId select b);
        db.Checkins.RemoveRange(
            from c in db.Checkins
            join s in db.Segments
            on c.Segment equals s.Id
            where s.Flight == flightId
            select c
        );

        flight.State = FlightState.Booking;
        return Save() ?? Results.Ok();
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

    public IResult SearchPromo(string code)
    {
        var promo = (from p in db.Promos where p.Code == code select p).SingleOrDefault();
        if (promo == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(promo.Id);
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
                         Id = segment.Id,
                         Flight = segment.Flight,
                         SeqNo = segment.SeqNo,
                         FromLoc = segment.FromLocNavigation.Code,
                         FromTime = segment.FromTime,
                         ToLoc = segment.ToLocNavigation.Code,
                         ToTime = segment.ToTime,
                         Aircraft = segment.Aircraft
                     };

        return Results.Ok(tagged.ToArray());
    }

    public IResult CheckIn(Guid segmentId, CheckIn checkIn)
    {
        var segment = (from s in db.Segments where s.Id == segmentId select s).SingleOrDefault();
        var pax = (from u in db.Users where u.Id == checkIn.Pax select u).SingleOrDefault();

        if (segment == null || pax == null)
        {
            return Results.NotFound();
        }

        var flight = segment.FlightNavigation;
        var seats = segment.AircraftNavigation.Seats;

        if (checkIn.Seat < 0 || checkIn.Seat >= seats || flight.State != FlightState.Checkin)
        {
            return Results.BadRequest();
        }

        var row = new Checkin
        {
            Segment = segmentId,
            Pax = checkIn.Pax,
            Seat = checkIn.Seat,
        };

        db.Checkins.Add(row);
        return Save() ?? Results.Ok();
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

        var results = from flight in flights.ToArray()
                      where flight.State == FlightState.Booking
                      select new SearchResult
                      {
                          Flight = flight,
                          Route = FlightRoute(flight),
                      };

        return Results.Ok(results.ToArray());
    }

    private TecAirContext db;
    private Random random = new Random();

    private Airport[] FlightRoute(Flight flight)
    {
        var query = from segment in db.Segments
                    where segment.Flight == flight.Id
                    orderby segment.SeqNo
                    select segment;

        var enumerator = query.ToList().GetEnumerator();

        var valid = enumerator.MoveNext();
        var route = new List<Airport>();

        while (valid)
        {
            var segment = enumerator.Current;
            route.Add(segment.FromLocNavigation);

            valid = enumerator.MoveNext();
            if (!valid)
            {
                route.Add(segment.ToLocNavigation);
            }
        }

        return route.ToArray();
    }

    private IResult? Save()
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

    private byte[] RandomSalt()
    {
        var salt = new byte[16];
        random.NextBytes(salt);
        return salt;
    }

    private byte[] HashFor(string password, byte[] salt)
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

public class CheckIn
{
    [Required]
    public Guid Pax { get; set; }
    [Required]
    public int Seat { get; set; }
}

public class NewBag
{
    [Required]
    public Guid Owner { get; set; }
    [Required]
    public decimal Weight { get; set; }
    [Required]
    public string Color { get; set; } = null!;
}

public class Booked
{
    public decimal Total { get; set; }
}

public class SearchResult
{
    public Flight Flight { get; set; } = null!;
    public Airport[] Route { get; set; } = null!;
}

public class TaggedSegment
{
    public Guid Id { get; set; }
    public Guid Flight { get; set; }
    public int SeqNo { get; set; }
    public String FromLoc { get; set; } = null!;
    public DateTimeOffset FromTime { get; set; }
    public String ToLoc { get; set; } = null!;
    public DateTimeOffset ToTime { get; set; }
    public Guid Aircraft { get; set; }
}

public class InsertedBag
{
    public Guid Id { get; set; }
    public int No { get; set; }
}
