using InventoryManagement.Core.Entities;
using InventoryManagement.Service.Common;
using InventoryManagement.Service.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventoryManagement.Service.Common;
namespace InventoryManagement.UI
{
    public partial class ProductForm : Form
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly User _currentUser;
        private int? _productId;

        public ProductForm(
          ProductService productService,
          CategoryService categoryService,
          User currentUser)
        {
            InitializeComponent();
            btnSave.Click += btnSave_Click;

            _productService = productService
                ?? throw new ArgumentNullException(nameof(productService));

            _categoryService = categoryService
                ?? throw new ArgumentNullException(nameof(categoryService));

            _currentUser = currentUser
                ?? throw new ArgumentNullException(nameof(currentUser));

            SetupLayout();
            LoadCategories();

        }

        public ProductForm(
            ProductService productService,
            CategoryService categoryService,
            User currentUser,
            Product product)
            : this(productService, categoryService, currentUser)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            _productId = product.Id;

            txtName.Text = product.Name;
            txtDescription.Text = product.Description;
            numQuantity.Value = product.Quantity;
            numPrice.Value = product.Price;
            txtSupplierName.Text = product.SupplierName;
            cmbCategory.SelectedValue = product.CategoryId;

            Text = "Edit Product";
        }
        private void SetupLayout()
        {
            Text = "Product Details";
            StartPosition = FormStartPosition.CenterParent;
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 10F);

            MinimumSize = new Size(600, 500);
            Size = new Size(760, 600);
            Padding = new Padding(16);

            // Allow the Description field to accept multiple lines.
            txtDescription.Multiline = true;
            txtDescription.ScrollBars = ScrollBars.Vertical;

            // Configure numeric inputs.
            numQuantity.Minimum = 0;
            numQuantity.Maximum = 1000000;
            numQuantity.DecimalPlaces = 0;

            numPrice.Minimum = 0;
            numPrice.Maximum = 100000000;
            numPrice.DecimalPlaces = 2;
            numPrice.ThousandsSeparator = true;
            SetupResponsiveLayout();
        }
        private void SetupResponsiveLayout()
        {
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(8),
            };

            mainLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 160));

            mainLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            for (int i = 0; i < 6; i++)
            {
                mainLayout.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 55));
            }

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            AddRow(mainLayout, 0, lblProductName, txtName);
            AddRow(mainLayout, 1, lblDescription, txtDescription);
            AddRow(mainLayout, 2, lblQuantity, numQuantity);
            AddRow(mainLayout, 3, lblPrice, numPrice);
            AddRow(mainLayout, 4, lblSupplierName, txtSupplierName);
            AddRow(mainLayout, 5, lblCategory, cmbCategory);

            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(0, 8, 0, 0)
            };

            btnSave.AutoSize = true;
            btnCancel.AutoSize = true;

            buttonsPanel.Controls.Add(btnCancel);
            buttonsPanel.Controls.Add(btnSave);

            mainLayout.Controls.Add(buttonsPanel, 1, 6);

            // Reparent existing controls into the new layout.
            Controls.Clear();
            Controls.Add(mainLayout);
        }

        private void AddRow(
            TableLayoutPanel layout,
            int row,
            Label label,
            Control input)
        {
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;

            input.Dock = DockStyle.Fill;
            input.Margin = new Padding(3, 5, 3, 5);

            layout.Controls.Add(label, 0, row);
            layout.Controls.Add(input, 1, row);
        }


        private void LoadCategories()
        {
            var result = _categoryService.GetActiveCategories();

            if (!result.IsSuccess)
            {
                MessageBox.Show(
                    result.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "Id";
            cmbCategory.DataSource = result.Data;

            cmbCategory.SelectedIndex = -1;
        }

        // Events

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbCategory.SelectedValue == null ||
                !int.TryParse(cmbCategory.SelectedValue.ToString(), out int categoryId) ||
                categoryId <= 0)
            {
                MessageBox.Show(
                    "Please select a valid category.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }
            bool isSuccess;
            string message;

            if (_productId.HasValue)
            {
                var result = _productService.UpdateProduct(
                    _productId.Value,
                    txtName.Text,
                    txtDescription.Text,
                    (int)numQuantity.Value,
                    numPrice.Value,
                    txtSupplierName.Text,
                    categoryId,
                    _currentUser.Id);

                isSuccess = result.IsSuccess;
                message = result.Message;
            }
            else
            {
                var result = _productService.AddProduct(
                    txtName.Text,
                    txtDescription.Text,
                    (int)numQuantity.Value,
                    numPrice.Value,
                    txtSupplierName.Text,
                    categoryId,
                    _currentUser.Id);

                isSuccess = result.IsSuccess;
                message = result.Message;
            }

            if (!isSuccess)
            {
                MessageBox.Show(
                    message,
                    "Unable to Save Product",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            MessageBox.Show(
                _productId.HasValue
                    ? "Product updated successfully."
                    : "Product added successfully.",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }

    }
}
