using System;
using System.Data;
using System.Linq;
using InventoryManagement.Service.Services;

namespace InventoryManagement.UI
{
    public class LowStockReportDataBuilder
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public LowStockReportDataBuilder(
            ProductService productService,
            CategoryService categoryService)
        {
            _productService = productService
                ?? throw new ArgumentNullException(nameof(productService));

            _categoryService = categoryService
                ?? throw new ArgumentNullException(nameof(categoryService));
        }
        public DataTable Build()
        {
            var productsResult =
                _productService.SearchProducts(null, null, true);

            if (!productsResult.IsSuccess)
            {
                throw new InvalidOperationException(
                    productsResult.Message ?? "Could not load low-stock products.");
            }

            var categoriesResult =
                _categoryService.GetActiveCategories();

            if (!categoriesResult.IsSuccess)
            {
                throw new InvalidOperationException(
                    categoriesResult.Message ?? "Could not load categories.");
            }

            var categories = categoriesResult.Data
                .ToDictionary(category => category.Id, category => category.Name);

            var table = new DataTable("LowStockProducts");

            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("CategoryName", typeof(string));
            table.Columns.Add("Quantity", typeof(int));
            table.Columns.Add("Price", typeof(decimal));
            table.Columns.Add("SupplierName", typeof(string));

            foreach (var product in productsResult.Data)
            {
                string categoryName;

                if (!categories.TryGetValue(product.CategoryId, out categoryName))
                {
                    categoryName = "Unavailable Category";
                }

                table.Rows.Add(
                    product.Id,
                    product.Name ?? string.Empty,
                    categoryName,
                    product.Quantity,
                    product.Price,
                    product.SupplierName ?? string.Empty);
            }

            return table;
        }

    }
}