using backend.Domain.Interfaces;
using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace backend.Infrastructure.Security
{
    public class Argon2PasswordHasher : IPasswordHasher
    {
        private readonly PasswordOptions _options;

        public Argon2PasswordHasher(PasswordOptions options)
        {
            _options = options;
        }

        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(_options.SaltSize);

            var hash = GenerateHash(password, salt);

            return Convert.ToBase64String(salt) + "." +
                   Convert.ToBase64String(hash);
        }

        public bool Verify(string password, string storedHash, out bool needsRehash)
        {
            needsRehash = false;

            var parts = storedHash.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var expectedHash = Convert.FromBase64String(parts[1]);

            var actualHash = GenerateHash(password, salt);

            var valid = CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);

            // exemplo simples de rehash policy
            needsRehash = _options.Iterations < 5;

            return valid;
        }

        private byte[] GenerateHash(string password, byte[] salt)
        {
            var argon2 = new Argon2id(
                Encoding.UTF8.GetBytes(password + _options.Pepper));

            argon2.Salt = salt;
            argon2.DegreeOfParallelism = _options.DegreeOfParallelism;
            argon2.Iterations = _options.Iterations;
            argon2.MemorySize = _options.MemorySizeKB;

            return argon2.GetBytes(_options.HashSize);
        }
    }
}
