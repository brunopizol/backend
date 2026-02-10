using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Application
{
    public static class PasswordPolicy
    {
        public static void Validate(string password)
        {
            if (password.Length < 12)
                throw new Exception("Password too short");

            if (!password.Any(char.IsUpper))
                throw new Exception("Missing uppercase");

            if (!password.Any(char.IsDigit))
                throw new Exception("Missing number");

            if (!password.Any(c => "!@#$%^&*".Contains(c)))
                throw new Exception("Missing special char");
        }
    }
}
