using System.Collections.Generic;
using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetActiveProducts();

        Product GetById(int id);

        List<Product> Search(
            string name,
            int? categoryId,
            bool? lowStockOnly);

        int Add(Product product);

        bool Update(Product product);

        bool SoftDelete(int id);

        List<Product> GetByCategoryId(int categoryId);

        bool HasActiveProducts(int categoryId);

        int SoftDeleteByCategoryId(int categoryId);
    }
}