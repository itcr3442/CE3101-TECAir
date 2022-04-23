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
            University = user.University,
            StudentId = user.StudentId,
            Type = user.Type,
        };

        db.Users.Add(row);
        return save() ?? Results.Ok(row.Id);
    }

    public IResult DeleteUser(Guid id)
    {
        db.Entry(new User { Id = id }).State = EntityState.Deleted;
        return save() ?? Results.Ok();
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
