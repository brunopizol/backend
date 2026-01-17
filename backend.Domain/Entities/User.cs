using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsActive { get; private set; }

        protected User() { }

        public User(string name, string email, string password)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Password = password;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;

            Validate();
        }

        public void Update(string name, string email)
        {
            Name = name;
            Email = email;
            UpdatedAt = DateTime.UtcNow;

            Validate();
        }

        public void ChangePassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Password cannot be empty");

            Password = newPassword;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        private void Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Name is required");

            if (string.IsNullOrWhiteSpace(Email))
                errors.Add("Email is required");
            else if (!Email.Contains("@"))
                errors.Add("Invalid email format");

            if (string.IsNullOrWhiteSpace(Password))
                errors.Add("Password is required");

            if (errors.Count > 0)
                throw new ArgumentException(string.Join(", ", errors));
        }
    }
}
