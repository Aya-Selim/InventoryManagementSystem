using InventoryManagement.Core.Models;

namespace InventoryManagement.Core.Interfaces
{
    public interface IPasswordHasher
    {
        PasswordHashResult HashPassword(string password);

        bool VerifyPassword(
            string password,
            byte[] hash,
            byte[] salt);
    }
}