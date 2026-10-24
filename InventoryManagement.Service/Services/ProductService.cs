
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Enums;
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Service.Common;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace InventoryManagement.Service.Services
{
    public class ProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        private readonly JavaScriptSerializer _serializer =
            new JavaScriptSerializer();

        public ProductService(
             IUnitOfWork unitOfWork,
             IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));

            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
        }

        // CREATE
        public ServiceResult<int> AddProduct(
            string name,
            string description,
            int quantity,
            decimal price,
            string supplierName,
            int categoryId,
            int userId)
        {

            var authorization = AuthorizeAdmin(userId);

            if (!authorization.IsSuccess)
                return ServiceResult<int>.Fail(authorization.Message);

            string validationError = ValidateProductInput(
                name, description, quantity, price,
                supplierName, categoryId, userId);

            if (validationError != null)
                return ServiceResult<int>.Fail(validationError);

            name = name.Trim();
            supplierName = supplierName.Trim();

            description = string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim();

            if (!IsActiveCategory(categoryId))
                return ServiceResult<int>.Fail(
                    "Selected category was not found.");

            var product = new Product
            {
                Name = name,
                Description = description,
                Quantity = quantity,
                Price = price,
                SupplierName = supplierName,
                CategoryId = categoryId,
                CreatedBy = userId,
                IsDeleted = false
            };

            _unitOfWork.BeginTransaction();

            try
            {
                int productId = _unitOfWork.Products.Add(product);

                var savedProduct =
                    _unitOfWork.Products.GetById(productId);

                if (savedProduct == null)
                    throw new InvalidOperationException(
                        "Product could not be retrieved after creation.");

                _unitOfWork.AuditLogs.Add(
                    CreateAuditLog(
                        "Create",
                        productId,
                        userId,
                        null,
                        SerializeProduct(savedProduct)));

                _unitOfWork.Commit();

                return ServiceResult<int>.Ok(productId);
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        // UPDATE
        public ServiceResult UpdateProduct(
            int id,
            string name,
            string description,
            int quantity,
            decimal price,
            string supplierName,
            int categoryId,
            int userId)
        {

            var authorization = AuthorizeAdmin(userId);

            if (!authorization.IsSuccess)
                return ServiceResult.Fail(authorization.Message);

            if (id <= 0)
                return ServiceResult.Fail("Invalid product ID.");

            string validationError = ValidateProductInput(
                name, description, quantity, price,
                supplierName, categoryId, userId);

            if (validationError != null)
                return ServiceResult.Fail(validationError);

            var existing = _unitOfWork.Products.GetById(id);

            if (existing == null)
                return ServiceResult.Fail("Product was not found.");

            if (!IsActiveCategory(categoryId))
                return ServiceResult.Fail(
                    "Selected category was not found.");

            // Capture the original state before changing the entity.
            string oldValues = SerializeProduct(existing);

            name = name.Trim();
            supplierName = supplierName.Trim();

            description = string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim();

            existing.Name = name;
            existing.Description = description;
            existing.Quantity = quantity;
            existing.Price = price;
            existing.SupplierName = supplierName;
            existing.CategoryId = categoryId;
            existing.UpdatedBy = userId;

            _unitOfWork.BeginTransaction();

            try
            {
                if (!_unitOfWork.Products.Update(existing))
                {
                    _unitOfWork.Rollback();

                    return ServiceResult.Fail(
                        "Product could not be updated.");
                }

                var updatedProduct =
                    _unitOfWork.Products.GetById(id);

                if (updatedProduct == null)
                    throw new InvalidOperationException(
                        "Updated product could not be retrieved.");

                _unitOfWork.AuditLogs.Add(
                    CreateAuditLog(
                        "Update",
                        id,
                        userId,
                        oldValues,
                        SerializeProduct(updatedProduct)));

                _unitOfWork.Commit();

                return ServiceResult.Ok();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        // SOFT DELETE
        public ServiceResult DeleteProduct(int id, int userId)
        {
            if (id <= 0)
                return ServiceResult.Fail("Invalid product ID.");

            if (userId <= 0)
                return ServiceResult.Fail("Invalid user.");

            var existing = _unitOfWork.Products.GetById(id);

            if (existing == null)
                return ServiceResult.Fail("Product was not found.");

            // Capture the original state before deletion.
            string oldValues = SerializeProduct(existing);

            _unitOfWork.BeginTransaction();

            try
            {
                if (!_unitOfWork.Products.SoftDelete(id))
                {
                    _unitOfWork.Rollback();

                    return ServiceResult.Fail(
                        "Product could not be deleted.");
                }

                // Represent the product's logical state after deletion.
                existing.IsDeleted = true;

                _unitOfWork.AuditLogs.Add(
                    CreateAuditLog(
                        "Delete",
                        id,
                        userId,
                        oldValues,
                        SerializeProduct(existing)));

                _unitOfWork.Commit();

                return ServiceResult.Ok();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public ServiceResult<List<Product>> SearchProducts(
            string name,
            int? categoryId,
            bool? lowStockOnly)
        {
            if (categoryId.HasValue && categoryId.Value <= 0)
            {
                return ServiceResult<List<Product>>.Fail(
                    "Invalid category.");
            }

            name = string.IsNullOrWhiteSpace(name)
                ? null
                : name.Trim();

            var products = _unitOfWork.Products.Search(
                name,
                categoryId,
                lowStockOnly);

            return ServiceResult<List<Product>>.Ok(products);
        }


        //-----------------------------------------------------------------------------------


        // VALIDATION


        private ServiceResult AuthorizeAdmin(int userId)
        {
            if (userId <= 0)
                return ServiceResult.Fail("Invalid user.");

            var user = _userRepository.GetById(userId);

            if (user == null || !user.IsActive)
                return ServiceResult.Fail(
                    "User was not found or is inactive.");

            if (user.Role != UserRole.Admin)
                return ServiceResult.Fail(
                    "You are not authorized to perform this action.");

            return ServiceResult.Ok();
        }

        private string ValidateProductInput(
            string name,
            string description,
            int quantity,
            decimal price,
            string supplierName,
            int categoryId,
            int userId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Product name is required.";

            if (name.Trim().Length > 150)
                return "Product name cannot exceed 150 characters.";

            if (description != null &&
                description.Trim().Length > 500)
            {
                return "Description cannot exceed 500 characters.";
            }

            if (quantity < 0)
                return "Quantity cannot be negative.";

            if (price < 0)
                return "Price cannot be negative.";

            if (string.IsNullOrWhiteSpace(supplierName))
                return "Supplier name is required.";

            if (supplierName.Trim().Length > 150)
                return "Supplier name cannot exceed 150 characters.";

            if (categoryId <= 0)
                return "Invalid category.";

            if (userId <= 0)
                return "Invalid user.";

            return null;
        }

        // CATEGORY VALIDATION
        private bool IsActiveCategory(int categoryId)
        {
            var category =
                _unitOfWork.Categories.GetById(categoryId);

            return category != null && !category.IsDeleted;
        }

        // AUDIT LOG FACTORY
        private AuditLog CreateAuditLog(
            string actionType,
            int productId,
            int userId,
            string oldValues,
            string newValues)
        {
            return new AuditLog
            {
                ActionType = actionType,
                EntityType = "Product",
                ProductId = productId,
                UserId = userId,
                OldValues = oldValues,
                NewValues = newValues
            };
        }

        // JSON SERIALIZATION
        private string SerializeProduct(Product product)
        {
            if (product == null)
                return null;

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
