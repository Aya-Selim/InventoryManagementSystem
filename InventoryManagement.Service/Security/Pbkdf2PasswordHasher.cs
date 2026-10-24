
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Core.Models;
using System.Security.Cryptography;

namespace InventoryManagement.Service.Security
{
    public class Pbkdf2PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public PasswordHashResult HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash;

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256))
            {
                hash = pbkdf2.GetBytes(HashSize);
            }

            return new PasswordHashResult(hash, salt);
        }

        public bool VerifyPassword(
            string password,
            byte[] hash,
            byte[] salt)
        {
            if (password == null || hash == null || salt == null)
            {
                return false;
            }

            if (hash.Length != HashSize || salt.Length != SaltSize)
            {
                return false;
            }

            byte[] computedHash;

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256))
            {
                computedHash = pbkdf2.GetBytes(HashSize);
            }

            int diff = 0;

            for (int i = 0; i < HashSize; i++)
            {
                diff |= hash[i] ^ computedHash[i];
            }

            return diff == 0;
        }
    }
}
