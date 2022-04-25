/* Capa de servicios de TECAir.
 *
 * Este archivo implementa la lógica de los endpoints utilizados por las
 * aplicaciones web y móvil.
 */
namespace backend;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Npgsql;
using backend.dal;

class ServiceLayer
{
    // Construye la capa de servicios dado un contexto de base de datos.
    public ServiceLayer(TecAirContext context)
    {
        db = context;
    }

    // Verifica si un par usuario-contraseña cnstituye credenciales válidas.
    public IResult CheckLogin(string username, string password)
    {
        var user = (from u in db.Users where u.Username == username select u).SingleOrDefault();
        if (user == null || user.Hash == null || user.Salt == null)
        {
            // En este caso, no se ha definido una contraseña
            return Results.Unauthorized();
        }

        // Se compara la prueba hash-sal contra lo almacenado
        var salt = Convert.FromHexString(user.Salt);
        var hash = Convert.FromHexString(user.Hash);
        var challenge = HashFor(password, salt);

        return hash.SequenceEqual(challenge) ? Results.Ok(user.Id) : Results.Unauthorized();
    }

    // Agrega un nuevo usuario con sus datos personales y una clave.
    public IResult AddUser(NewUser user)
    {
        // Criptográficamente, la sal debe ser idealmente aleatoria.
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
            Miles = 0,
        };

        db.Users.Add(row);
        return Save() ?? Results.Ok(row.Id); // Retorna el UUID de la nueva tupla
    }

    // Obtiene parámetros de un usuario dado su identificador único.
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
            Hash = null, // <--
            Salt = null, // <-- Nótese que se descarta información sensible
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phonenumber = user.Phonenumber,
            Email = user.Email,
            University = user.University,
            StudentId = user.StudentId,
            Type = user.Type,
            Miles = user.Miles,
        };

        return Results.Ok(plain);
    }

    /* Actualiza un usuario.
     *
     * El parámetro `id` es el UUID del usuario a actualizar y `edit` contiene
     * la información a actualizar. Los elementos de `edit` que sean nulos se
     * ignoran.
     */
    public IResult UpdateUser(Guid id, EditUser edit)
    {
        var user = (from u in db.Users where u.Id == id select u).SingleOrDefault();
        if (user == null)
        {
            return Results.NotFound();
        }

        user.Username = edit.Username ?? user.Username;

        // Recálculo de par sal-hash en caso de cambio de contraseña
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

    // Elimina un usuario del sistema.
    public IResult DeleteUser(Guid id)
    {
        // Primero deben eliminarse las referencias al usuario en otras tablas.
        db.Bookings.RemoveRange(from b in db.Bookings where b.Pax == id select b);
        db.Bags.RemoveRange(from b in db.Bags where b.Owner == id select b);
        db.Checkins.RemoveRange(from c in db.Checkins where c.Pax == id select c);
        db.Entry(new User { Id = id }).State = EntityState.Deleted;

        return Save() ?? Results.Ok();
    }

    /* Dado el UUID de un pasajero, obtiene la lista de UUIDs de aquellos vuelos
     * que pasajero haya reservado y que actualmente se encuentren abiertos.
     */
    public IResult GetOpenFlights(Guid paxId)
    {
        var user = db.Users.Where(u => u.Id == paxId).SingleOrDefault();
        if (user == null)
        {
            return Results.NotFound();
        }

        var ids = from booking in db.Bookings
                  where booking.Pax == paxId && booking.FlightNavigation.State == FlightState.Checkin
                  select booking.FlightNavigation.Id;

        return Results.Ok(ids.ToArray());
    }

    /* Añaade un nuevo vuelo.
     *
     * El objeto parámetro contiene la lista de segmentos, aeronaves y
     * aeropuertos que constarán el vuelo. Esta operación puede fallar si
     * cualquiera de los identificadores es incorrecto o si existe una colisión
     * de identidad con respecto al número de vuelo.
     */
    public IResult AddFlight(NewFlight newFlight)
    {
        if (newFlight.Segments.Length == 0 || newFlight.Airports.Length != newFlight.Segments.Length + 1)
        {
            return Results.BadRequest();
        }

        // Verificación de integridad
        foreach (var airport in newFlight.Airports)
        {
            if (db.Airports.Where(a => a.Id == airport).SingleOrDefault() == null)
            {
                return Results.NotFound();
            }
        }

        // Verificación de integridad
        foreach (var segment in newFlight.Segments)
        {
            if (db.Aircraft.Where(a => a.Id == segment.Aircraft).SingleOrDefault() == null)
            {
                return Results.NotFound();
            }
        }

        var row = new Flight
        {
            No = newFlight.No,
            Price = newFlight.Price,
            Comment = newFlight.Comment,
            State = FlightState.Booking,
        };

        db.Flights.Add(row);

        var result = Save();
        if (result != null)
        {
            return result;
        }

        // Se crean los segmentos del vuelo
        for (var i = 0; i < newFlight.Segments.Length; ++i)
        {
            var segment = new Segment
            {
                Flight = row.Id,
                SeqNo = i,
                FromLoc = newFlight.Airports[i],
                FromTime = newFlight.Segments[i].FromTime,
                ToLoc = newFlight.Airports[i + 1],
                ToTime = newFlight.Segments[i].ToTime,
                Aircraft = newFlight.Segments[i].Aircraft,
            };

            db.Segments.Add(segment);
        }

        return Save() ?? Results.Ok(row.Id);
    }

    /* Dado un identificador de vuelo, obtiene el vuelo y la
     * constitución de su ruta.
     */
    public IResult GetFlight(Guid flightId)
    {
        var flight = db.Flights.Where(f => f.Id == flightId).SingleOrDefault();
        if (flight == null)
        {
            return Results.NotFound();
        }

        var result = FlightRoute(flight);
        return flight != null ? Results.Ok(result) : Results.NotFound();
    }

    // Elimina un vuelo a partir de su UUID.
    public IResult DeleteFlight(Guid flightId)
    {
        var flight = db.Flights.Where(f => f.Id == flightId).SingleOrDefault();
        if (flight == null)
        {
            return Results.NotFound();
        }

        // Deben descartarse las referencias al vuelo
        db.Bookings.RemoveRange(db.Bookings.Where(b => b.Flight == flightId));
        db.Bags.RemoveRange(db.Bags.Where(b => b.Flight == flightId));
        db.Promos.RemoveRange(db.Promos.Where(p => p.Flight == flightId));

        // Se eliminan segmentos y sus checkins asociados
        foreach (var segment in db.Segments.Where(s => s.Flight == flightId).ToArray())
        {
            db.Checkins.RemoveRange(db.Checkins.Where(c => c.Segment == segment.Id));
            db.Segments.Remove(segment);
        }

        // Finalmente, se puede eliminar el vuelo
        db.Flights.Remove(flight);
        return Save() ?? Results.Ok();
    }

    /* Crea una nueva reservación de vuelo.
     *
     * este método toma el identificador del vuelo a reservar y
     * una descripción de la reservación.
     *
     * El vuelo debe estar en estado de reservación y además el
     * pasajero no debe haber reservado este vuelo en particular
     * todavqa.
     */
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

        // Aplicación de la promoción.
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

		// Solo se agregan millas a estudiantes
		if (pax.StudentId != null)
		{
        	pax.Miles += 1;
		}

        db.Bookings.Add(new Booking { Flight = flightId, Pax = paxId, Promo = promoId });
        return Save() ?? Results.Ok(new Booked { Total = total });
    }

    // Registra una maleta para un pasajero que se encuentra chequeado.
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

        // Restricciones
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

        // El pasajero debe estar chequeado
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

    // Abre un vuelo (habilita checkin).
    //
    // Para que esto sea permitido, el vuelo debe estar en booking o checkin.
    // Una vez que se inicia este proceso, ya no es posible hacer booking nuevamente.
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

    // Cierra un vuelo.
    //
    // La lógica es análoga a abrir un vuelo, con la diferencia que al momento
    // de cerrar se genera un resumen de listado de maletas por pasajero que se
    // utiliza para emitir un reporte.
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
                break;

            case FlightState.Checkin:
                flight.State = FlightState.Closed;
                break;

            default:
                return Results.BadRequest();
        }

        var paxes = from checkin in db.Checkins
                    join segment in db.Segments
                    on checkin.Segment equals segment.Id
                    where segment.Flight == flightId
                    select checkin.PaxNavigation;

        // Cuenta maletas por cada pasajero chequeado (no cuentan pasajeros no chequeados)
        var close = new List<PaxClose>();
        foreach (var pax in paxes.Distinct().ToArray())
        {
            var paxClose = new PaxClose
            {
                Pax = pax.Id,
                Bags = db.Bags.Where(b => b.Flight == flightId && b.Owner == pax.Id).Count(),
            };

            close.Add(paxClose);
        }

        return Save() ?? Results.Ok(close);
    }

    // Devuelve un vuelo al estado inicial (booking).
    public IResult ResetFlight(Guid flightId)
    {
        var flight = (from f in db.Flights where f.Id == flightId select f).SingleOrDefault();
        if (flight == null)
        {
            return Results.NotFound();
        }

        // Elimina todos los bookings y checkins existentes.
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

    // Retorna todos los usuarios en el sistema.
    public IResult DumpUsers()
    {
        var users = db.Users.ToArray();
        foreach (var user in users)
        {
            // Descarta información sensible
            user.Hash = null;
            user.Salt = null;
        }

        return Results.Ok(users);
    }

    /* Retorna todos los vuelos en el sistema.
     *
     * Si "filterBooking" es true, solo se consideran los vuelos
     */
    public IResult DumpFlights(bool filterBooking)
    {
        IEnumerable<Flight> flights = db.Flights;
        if (filterBooking)
        {
            flights = flights.Where(f => f.State == FlightState.Booking);
        }

        return Results.Ok(flights.ToArray());
    }

    // Retorna todas las promos en el sistema.
    public IResult DumpPromos()
    {
        return Results.Ok(db.Promos.ToArray());
    }

    // Reporta los aeropuertos.
    public IResult DumpAirports()
    {
        return Results.Ok(db.Airports.ToArray());
    }

    // Reporta las aeronaves.
    public IResult DumpAircraft()
    {
        return Results.Ok(db.Aircraft.ToArray());
    }

    // Busca el UUID de una promoción a partir del código de promoción.
    public IResult SearchPromo(string code)
    {
        var promo = (from p in db.Promos where p.Code == code select p).SingleOrDefault();
        if (promo == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(promo.Id);
    }

    // Elimina una promoción.
    public IResult DeletePromo(Guid promoId)
    {
        var promo = db.Promos.Where(p => p.Id == promoId).SingleOrDefault();
        if (promo == null)
        {
            return Results.NotFound();
        }

        // Se eliminan referencias a la promoción antes de eliminarla
        foreach (var booking in db.Bookings.Where(b => b.Promo == promoId))
        {
            booking.Promo = null;
        }

        db.Promos.Remove(promo);
        return Save() ?? Results.Ok();
    }

    // Vuelca todos los segmentos en el sistema.
    //
    // Si `filterBooking` es true, se filtra para solo mostrar los
    // segmentos asociados a vuelos que están actualmente aceptando
    // reservaciones.
    public IResult DumpSegments(bool filterBooking)
    {
        var segments = db.Segments.ToList();
        if (filterBooking)
        {
            segments = segments.Where(s => s.FlightNavigation.State == FlightState.Booking).ToList();
        }

        /* Esta expresión construye una descripción del segmento junto
         * a información descriptiva que se encuentra en otras tabla para
         * formar la salida.
         */
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

    /* Chequea un pasajero en un segmento de vuelo.
     *
     * El chequeo requiere que el vuelo se haya abierto y además que
     * el pasajero no haya realizado esta operación todavía en el segmento
     * particular. Se considera que un pax se ha chequeado en un vuelo si
     * se ha chequeado en al menos uno de los segmentos del vuelo. Los asientos
     * de vuelo se toman en cuenta y no se permite la repitición de estos.
     */
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

    /* Dada una cadena de origen y una de destino, busca vuelos
     * que contengan a una subruta que inicia en ese origen y termina
     * en ese destino.
     *
     * Retorna el mismo tipo de resultados que GetFlight(), véase ese
     * método para detalles.
     */
    public IResult SearchFlights(string fromLoc, string toLoc)
    {
        var fromPort = (from a in db.Airports where a.Code == fromLoc select a).SingleOrDefault();
        var toPort = (from a in db.Airports where a.Code == toLoc select a).SingleOrDefault();

        // No hay resultados si los aeropuertos no existen
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
                      select FlightRoute(flight);

        return Results.Ok(results.ToArray());
    }

    private TecAirContext db;
    private Random random = new Random();

    /* Construye una descripción de la ruta de un vuelo.
     *
     * Esta operación se utiliza de manera general para describir por
     * completo a un vuelo. La respuesta incluye al vuelo en sí, todos
     * sus segmentos, todas las paradas y datos auxiliares (comentarios,
     * tiempos, etc) que son de interés inmediato para las aplicaciones
     * clientes.
     */
    private SearchResult FlightRoute(Flight flight)
    {
        var query = from segment in db.Segments
                    where segment.Flight == flight.Id
                    orderby segment.SeqNo
                    select segment;

        var enumerator = query.ToList().GetEnumerator();

        var route = new List<Airport>();
        var segments = new List<SegmentSeats>();

        var valid = enumerator.MoveNext();
        while (valid)
        {
            var segment = enumerator.Current;

            // Unavail enumera los asientos ocupados
            var unavail = from checkin in db.Checkins
                          where checkin.Segment == segment.Id
                          select checkin.Seat;

            var segmentSeatInfo = new SegmentSeats
            {
                Unavail = unavail.ToArray(),
                Id = segment.Id,
                FromTime = segment.FromTime,
                ToTime = segment.ToTime,
                AircraftCode = segment.AircraftNavigation.Code,
                Seats = segment.AircraftNavigation.Seats,
            };

            segments.Add(segmentSeatInfo);
            route.Add(segment.FromLocNavigation);

            valid = enumerator.MoveNext();
            if (!valid)
            {
                route.Add(segment.ToLocNavigation);
            }
        }

        return new SearchResult
        {
            Flight = new Flight
            {
                Id = flight.Id,
                No = flight.No,
                State = flight.State,
                Comment = flight.Comment,
                Price = flight.Price,
            },
            Route = route.ToArray(),
            Segments = segments.ToArray(),
        };
    }

    /* Intenta guardar los cambios realizados a la base de datos.
     * En caso de NO ocurrir errores, retorna nulo. Si hay un error
     * "esperable" (generalmente colisión de llaves primarias) trata
     * de traducir esto a una respuesta apropiada. La intención es
     * utilizar este método de la siguiente manera:
     *
     *   return Save() ?? Results.Ok(...);
     *
     * Lo cual simplifica el manejo de errores.
     */
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

    // Genera una sal aleatoria de 16 bytes
    private byte[] RandomSalt()
    {
        var salt = new byte[16];
        random.NextBytes(salt);
        return salt;
    }

    // Calcula el campo de hash de clave a partir del plaintext y la sal 
    private byte[] HashFor(string password, byte[] salt)
    {
        return KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 1000, 16);
    }
}

/* Las siguientes clases definen el formato de datos de la API
 * y se utilizan en los endpoints anteriores.
 */

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

public class NewFlight
{
    [Required]
    public int No { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public string? Comment { get; set; } = null!;
    [Required]
    public NewSegment[] Segments { get; set; } = null!;
    [Required]
    public Guid[] Airports { get; set; } = null!;
}

public class NewSegment
{
    [Required]
    public Guid Aircraft { get; set; }
    [Required]
    public DateTimeOffset FromTime { get; set; }
    [Required]
    public DateTimeOffset ToTime { get; set; }
}

public class Booked
{
    public decimal Total { get; set; }
}

public class SearchResult
{
    public Flight Flight { get; set; } = null!;
    public Airport[] Route { get; set; } = null!;
    public SegmentSeats[] Segments { get; set; } = null!;
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

public class SegmentSeats
{
    public Guid Id { get; set; }
    public DateTimeOffset FromTime { get; set; }
    public DateTimeOffset ToTime { get; set; }
    public string AircraftCode { get; set; } = null!;
    public int Seats { get; set; }
    public int[] Unavail { get; set; } = null!;
}

public class InsertedBag
{
    public Guid Id { get; set; }
    public int No { get; set; }
}

public class PaxClose
{
    public Guid Pax { get; set; }
    public int Bags { get; set; }
}
