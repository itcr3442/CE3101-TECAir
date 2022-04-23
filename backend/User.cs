using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace backend
{
    public partial class User
    {
        public User()
        {
            Bags = new HashSet<Bag>();
        }

        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string? Hash { get; set; }
        public string? Salt { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string? Phonenumber { get; set; }
        public string? Email { get; set; }
        public string? University { get; set; }
        public string? StudentId { get; set; }

        public virtual ICollection<Bag> Bags { get; set; }
    }
}
