using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Enums;
using InventoryManagement.Service.Services;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace InventoryManagement.UI
{
    public partial class ProductsForm : Form
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly User _currentUser;

        public ProductsForm(
          ProductService productService,
          CategoryService categoryService,
          User currentUser)
        {
            InitializeComponent();
            SetupLayout();
            SetupResponsiveLayout();
            // Event handlers
            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;

            _productService = productService
                ?? throw new ArgumentNullException(nameof(productService));

            _categoryService = categoryService
                ?? throw new ArgumentNullException(nameof(categoryService));

            _currentUser = currentUser
                ?? throw new ArgumentNullException(nameof(currentUser));

            cmbStockFilter.Items.Add("All");
            cmbStockFilter.Items.Add("Low Stock");
            cmbStockFilter.Items.Add("In Stock");
            cmbStockFilter.SelectedIndex = 0;


            LoadCategories();
            LoadProducts();


            cmbCategory.SelectedIndexChanged += (s, e) => LoadProducts();
            cmbStockFilter.SelectedIndexChanged += (s, e) => LoadProducts();
            txtSearch.TextChanged += (s, e) => LoadProducts();

            bool isAdmin = IsAdmin();

            btnAdd.Visible = isAdmin;
            btnEdit.Visible = isAdmin;
            btnDelete.Visible = isAdmin;

        }


        private bool IsAdmin()
        {
            return _currentUser.Role == UserRole.Admin;
        }
        private void LoadCategories()
        {
            var result = _categoryService.GetActiveCategories();

            if (!result.IsSuccess)
            {
                MessageBox.Show(
                    result.Message,
                    "Categories",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var categories = new System.Collections.Generic.List<Category>
            {
                new Category
                {
                    Id = 0,
                    Name = "All Categories"
                }
            };

            categories.AddRange(result.Data);

            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "Id";
            cmbCategory.DataSource = categories;
            cmbCategory.SelectedValue = 0;
        }


        private void LoadProducts()
        {
            int? categoryId = null;

            if (cmbCategory.SelectedValue != null)
            {
                int selectedCategoryId =
                    Convert.ToInt32(cmbCategory.SelectedValue);

                if (selectedCategoryId != 0)
                {
                    categoryId = selectedCategoryId;
                }
            }

            bool? lowStockOnly = null;

            if (cmbStockFilter.SelectedItem != null)
            {
                string selectedStockFilter =
                    cmbStockFilter.SelectedItem.ToString();

                if (selectedStockFilter == "Low Stock")
                {
                    lowStockOnly = true;
                }
                else if (selectedStockFilter == "In Stock")
                {
                    lowStockOnly = false;
                }
            }

            string productName = string.IsNullOrWhiteSpace(txtSearch.Text)
                ? null
                : txtSearch.Text.Trim();

            var result = _productService.SearchProducts(
                productName,
                categoryId,
                lowStockOnly);

            if (!result.IsSuccess)
            {
                MessageBox.Show(
                    result.Message,
                    "Products",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            dgvProducts.AutoGenerateColumns = true;
            dgvProducts.DataSource = null;
            dgvProducts.DataSource = result.Data;

            if (dgvProducts.Columns["IsDeleted"] != null)
                dgvProducts.Columns["IsDeleted"].Visible = false;

            if (dgvProducts.Columns["CreatedBy"] != null)
                dgvProducts.Columns["CreatedBy"].Visible = false;

            if (dgvProducts.Columns["UpdatedBy"] != null)
                dgvProducts.Columns["UpdatedBy"].Visible = false;


            if (dgvProducts.Columns["Id"] != null)
                dgvProducts.Columns["Id"].HeaderText = "Product ID";

            if (dgvProducts.Columns["Name"] != null)
                dgvProducts.Columns["Name"].HeaderText = "Product Name";

            if (dgvProducts.Columns["Description"] != null)
                dgvProducts.Columns["Description"].HeaderText = "Description";

            if (dgvProducts.Columns["Quantity"] != null)
                dgvProducts.Columns["Quantity"].HeaderText = "Quantity in Stock";

            if (dgvProducts.Columns["Price"] != null)
                dgvProducts.Columns["Price"].HeaderText = "Price";

            if (dgvProducts.Columns["SupplierName"] != null)
                dgvProducts.Columns["SupplierName"].HeaderText = "Supplier Name";

            if (dgvProducts.Columns["CategoryId"] != null)
                dgvProducts.Columns["CategoryId"].HeaderText = "Category ID";

            if (dgvProducts.Columns["CreatedAt"] != null)
                dgvProducts.Columns["CreatedAt"].HeaderText = "Created At";

            if (dgvProducts.Columns["UpdatedAt"] != null)
                dgvProducts.Columns["UpdatedAt"].HeaderText = "Updated At";
        }

        //Events :
        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new ProductForm(
                _productService,
                _categoryService,
                _currentUser))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    LoadProducts();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null)
            {
                MessageBox.Show(
                    "Please select a product to edit.",
                    "No Product Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var product = dgvProducts.CurrentRow.DataBoundItem as Product;

            if (product == null)
                return;

            using (var form = new ProductForm(
                _productService,
                _categoryService,
                _currentUser,
                product))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    LoadProducts();
                }
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null)
            {
                MessageBox.Show(
                    "Please select a product to delete.",
                    "No Product Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var product = dgvProducts.CurrentRow.DataBoundItem as Product;

            if (product == null)
                return;

            var confirmation = MessageBox.Show(
                $"Are you sure you want to delete '{product.Name}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmation != DialogResult.Yes)
                return;

            var result = _productService.DeleteProduct(
                product.Id,
                _currentUser.Id);

            if (!result.IsSuccess)
            {
                MessageBox.Show(
                    result.Message,
                    "Delete Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            MessageBox.Show(
                "Product deleted successfully.",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            LoadProducts();
        }


        // Layout setup method
        private void SetupLayout()
        {
            Text = "Products Management";
            StartPosition = FormStartPosition.CenterScreen;

            MinimumSize = new Size(850, 500);
            Size = new Size(1100, 650);

            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 10F);
        }

        private void SetupResponsiveLayout()
        {
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12)
            };

            mainLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));

            // Search and filters
            var filtersPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 0, 0, 10)
            };

            ConfigureFilterControl(lblSearch, 85);
            ConfigureFilterControl(txtSearch, 180);
            ConfigureFilterControl(lblCategory, 75);
            ConfigureFilterControl(cmbCategory, 160);
            ConfigureFilterControl(cmbStockFilter, 130);

            filtersPanel.Controls.Add(lblSearch);
            filtersPanel.Controls.Add(txtSearch);
            filtersPanel.Controls.Add(lblCategory);
            filtersPanel.Controls.Add(cmbCategory);
            filtersPanel.Controls.Add(cmbStockFilter);

            // Products grid
            dgvProducts.Dock = DockStyle.Fill;
            dgvProducts.Margin = new Padding(0, 5, 0, 10);
            dgvProducts.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            // Action buttons
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = true,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 8, 0, 0)
            };

            ConfigureActionButton(btnAdd);
            ConfigureActionButton(btnEdit);
            ConfigureActionButton(btnDelete);

            buttonsPanel.Controls.Add(btnDelete);
            buttonsPanel.Controls.Add(btnEdit);
            buttonsPanel.Controls.Add(btnAdd);

            // Move existing controls into the new layout.
            Controls.Clear();

            mainLayout.Controls.Add(filtersPanel, 0, 0);
            mainLayout.Controls.Add(dgvProducts, 0, 1);
            mainLayout.Controls.Add(buttonsPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ConfigureFilterControl(Control control, int width)
        {
            control.Width = width;
            control.Height = 30;
            control.Margin = new Padding(4, 6, 8, 6);

            if (control is Label label)
            {
                label.AutoSize = false;
                label.TextAlign = ContentAlignment.MiddleLeft;
            }
        }

        private void ConfigureActionButton(Button button)
        {
            button.Size = new Size(145, 36);
            button.Margin = new Padding(6, 4, 0, 4);
        }


    }
}
