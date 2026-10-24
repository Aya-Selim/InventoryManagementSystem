using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces
{
    public interface IUserRepository
    {
        bool HasAnyUsers();

        User GetByEmail(string email);

        bool EmailExists(string email);

        int Add(User user);
        User GetById(int id);
    }
}