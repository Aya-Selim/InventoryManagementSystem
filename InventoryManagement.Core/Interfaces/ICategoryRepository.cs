using System.Collections.Generic;
using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces
{
    public interface ICategoryRepository
    {
        List<Category> GetActiveCategories();

        Category GetById(int id);

        bool ExistsByName(string name);

        bool ExistsByName(string name, int excludeCategoryId);

        int Add(Category category);

        bool Update(Category category);

        bool SoftDelete(int id);
    }
}