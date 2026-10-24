
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Service.Common;

namespace InventoryManagement.Service.Services
{
    public class CategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JavaScriptSerializer _serializer =
            new JavaScriptSerializer();

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public ServiceResult<int> AddCategory(
            string name, string description, int userId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResult<int>.Fail(
                    "Category name is required.");

            name = name.Trim();

            if (name.Length > 100)
                return ServiceResult<int>.Fail(
                    "Category name cannot exceed 100 characters.");

            if (description != null && description.Trim().Length > 500)
                return ServiceResult<int>.Fail(
                    "Description cannot exceed 500 characters.");

            if (_unitOfWork.Categories.ExistsByName(name))
                return ServiceResult<int>.Fail(
                    "A category with this name already exists.");
            var category = new Category
            {
                Name = name,
                Description = string.IsNullOrWhiteSpace(description)
                    ? null
                    : description.Trim()
            };

            _unitOfWork.BeginTransaction();

            try
            {
                int categoryId = _unitOfWork.Categories.Add(category);

                var savedCategory =
                    _unitOfWork.Categories.GetById(categoryId);

                _unitOfWork.AuditLogs.Add(new AuditLog
                {
                    ActionType = "Create",
                    EntityType = "Category",
                    CategoryId = categoryId,
                    UserId = userId,
                    OldValues = null,
                    NewValues = SerializeCategory(savedCategory)
                });

                _unitOfWork.Commit();

                return ServiceResult<int>.Ok(categoryId);
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public ServiceResult<List<Category>> GetActiveCategories()
        {
            return ServiceResult<List<Category>>.Ok(
                _unitOfWork.Categories.GetActiveCategories());
        }

        public ServiceResult<Category> GetCategoryById(int id)
        {
            if (id <= 0)
                return ServiceResult<Category>.Fail(
                    "Invalid category ID.");

            var category = _unitOfWork.Categories.GetById(id);

            if (category == null || category.IsDeleted)
                return ServiceResult<Category>.Fail(
                    "Category was not found.");

            return ServiceResult<Category>.Ok(category);
        }

        public ServiceResult UpdateCategory(
            int id, string name, string description, int userId)
        {
            if (id <= 0)
                return ServiceResult.Fail("Invalid category ID.");

            if (string.IsNullOrWhiteSpace(name))
                return ServiceResult.Fail(
                    "Category name is required.");

            name = name.Trim();

            if (name.Length > 100)
                return ServiceResult.Fail(
                    "Category name cannot exceed 100 characters.");

            if (description != null && description.Trim().Length > 500)
                return ServiceResult.Fail(
                    "Description cannot exceed 500 characters.");

            var existing = _unitOfWork.Categories.GetById(id);

            if (existing == null || existing.IsDeleted)
                return ServiceResult.Fail("Category was not found.");

            if (_unitOfWork.Categories.ExistsByName(name, id))
                return ServiceResult.Fail(
                    "A category with this name already exists.");

            string oldValues = SerializeCategory(existing);

            existing.Name = name;
            existing.Description = string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim();

            _unitOfWork.BeginTransaction();

            try
            {
                if (!_unitOfWork.Categories.Update(existing))
                {
                    _unitOfWork.Rollback();

                    return ServiceResult.Fail(
                        "Category could not be updated.");
                }

                var updated = _unitOfWork.Categories.GetById(id);

                _unitOfWork.AuditLogs.Add(new AuditLog
                {
                    ActionType = "Update",
                    EntityType = "Category",
                    CategoryId = id,
                    UserId = userId,
                    OldValues = oldValues,
                    NewValues = SerializeCategory(updated)
                });

                _unitOfWork.Commit();

                return ServiceResult.Ok();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public ServiceResult DeleteCategory(int id, int userId)
        {
            if (id <= 0)
                return ServiceResult.Fail("Invalid category ID.");

            // Capture the original category before changing it.
            var category = _unitOfWork.Categories.GetById(id);

            if (category == null || category.IsDeleted)
                return ServiceResult.Fail("Category was not found.");

            // Capture active products before soft-deleting them.
            var products = _unitOfWork.Products.GetByCategoryId(id);

            string oldCategoryValues = SerializeCategory(category);
            var oldProductValues = products.ToDictionary(
                p => p.Id, p => SerializeProduct(p));

            _unitOfWork.BeginTransaction();

            try
            {
                // Soft-delete the category.
                if (!_unitOfWork.Categories.SoftDelete(id))
                    throw new InvalidOperationException(
                        "Category could not be deleted.");

                // Soft-delete all active products in this category.
                int deletedCount =
                    _unitOfWork.Products.SoftDeleteByCategoryId(id);

                if (deletedCount != products.Count)
                    throw new InvalidOperationException(
                        "The product list changed during deletion. Try again.");

                // Record the category deletion.
                var deletedCategory = _unitOfWork.Categories.GetById(id);

                _unitOfWork.AuditLogs.Add(new AuditLog
                {
                    ActionType = "Delete",
                    EntityType = "Category",
                    CategoryId = id,
                    UserId = userId,
                    OldValues = oldCategoryValues,
                    NewValues = SerializeCategory(deletedCategory)
                });

                // Record each affected product.
                foreach (var product in products)
                {
                    
                    product.IsDeleted = true;
                    product.UpdatedAt = DateTime.Now;

                    _unitOfWork.AuditLogs.Add(new AuditLog
                    {
                        ActionType = "Delete",
                        EntityType = "Product",
                        ProductId = product.Id,
                        UserId = userId,
                        OldValues = oldProductValues[product.Id],
                        NewValues = SerializeProduct(product)
                    });
                }

                _unitOfWork.Commit();
                return ServiceResult.Ok();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        private string SerializeCategory(Category category)
        {
            if (category == null)
                return null;

            return _serializer.Serialize(new
            {
                category.Id,
                category.Name,
                category.Description,
                category.IsDeleted
            });
        }

        private string SerializeProduct(Product product)
        {
            return _serializer.Serialize(new
            {
                product.Id,
                product.Name,
                product.Description,
                product.Quantity,
                product.Price,
                product.SupplierName,
                product.CategoryId,
                product.IsDeleted
            });
        }
    }
}
    