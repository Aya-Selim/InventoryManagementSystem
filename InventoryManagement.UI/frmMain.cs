
using System;
using System.Drawing;
using System.Windows.Forms;
using InventoryManagement.Core.Entities;
using InventoryManagement.Service.Services;

namespace InventoryManagement.UI
{
    public partial class frmMain : Form
    {
        private readonly User _currentUser;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        private readonly Color primaryColor =
            Color.FromArgb(23, 54, 93);

        private readonly Color backgroundColor =
            Color.FromArgb(244, 246, 250);

        public frmMain(
            User user,
            ProductService productService,
            CategoryService categoryService)
        {
            InitializeComponent();

            _currentUser = user
                ?? throw new ArgumentNullException(nameof(user));

            _productService = productService
                ?? throw new ArgumentNullException(nameof(productService));

            _categoryService = categoryService
                ?? throw new ArgumentNullException(nameof(categoryService));

            Text = "Inventory Management";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(800, 550);
            Size = new Size(1100, 700);
            BackColor = backgroundColor;
            Font = new Font("Segoe UI", 10F);
            AutoScaleMode = AutoScaleMode.Font;

            BuildDashboard();
        }

        private void BuildDashboard()
        {
            // Main layout
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };

            mainLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 85F));

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            // Header
            var header = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = primaryColor,
                Padding = new Padding(28, 0, 28, 0),
                Margin = Padding.Empty
            };

            var appTitle = new Label
            {
                Text = "Inventory Management",
                Dock = DockStyle.Left,
                Width = 400,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var userInfo = new Label
            {
                Text = $"{_currentUser.FirstName}  |  {_currentUser.Role}",
                Dock = DockStyle.Right,
                Width = 260,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight
            };

            header.Controls.Add(userInfo);
            header.Controls.Add(appTitle);

            // Dashboard content
            var content = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(35),
                BackColor = backgroundColor,
                AutoScroll = true
            };

            var contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 1,
                RowCount = 4,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            contentLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            contentLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48F));

            contentLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 55F));

            contentLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 230F));
            contentLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 230F));

            var dashboardTitle = new Label
            {
                Text = "Dashboard",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.FromArgb(23, 43, 77),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var welcomeText = new Label
            {
                Text = "Welcome back! Manage your inventory from here.",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(86, 101, 122),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Products card
            var productsCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(24),
                Margin = new Padding(0, 8, 0, 8)
            };

            var cardLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };

            cardLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            cardLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 42F));

            cardLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            cardLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48F));

            var productsTitle = new Label
            {
                Text = "Products",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Color.FromArgb(23, 43, 77),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var productsDescription = new Label
            {
                Text = "View, search, filter, and manage inventory products.",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(86, 101, 122),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var openProductsButton = new Button
            {
                Text = "Open Products  →",
                Dock = DockStyle.Right,
                Width = 190,
                Height = 42,
                FlatStyle = FlatStyle.Flat,
                BackColor = primaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            openProductsButton.FlatAppearance.BorderSize = 0;
            openProductsButton.Click += btnProducts_Click;

            cardLayout.Controls.Add(productsTitle, 0, 0);
            cardLayout.Controls.Add(productsDescription, 0, 1);
            cardLayout.Controls.Add(openProductsButton, 0, 2);

            productsCard.Controls.Add(cardLayout);
            // Low Stock Report card
            var reportCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(24),
                Margin = new Padding(0, 8, 0, 8)
            };

            var reportCardLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };

            reportCardLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            reportCardLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 42F));

            reportCardLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            reportCardLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48F));

            var reportTitle = new Label
            {
                Text = "Low Stock Report",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Color.FromArgb(23, 43, 77),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var reportDescription = new Label
            {
                Text = "View products that have reached the low-stock threshold.",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(86, 101, 122),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var openReportButton = new Button
            {
                Text = "Open Report  →",
                Dock = DockStyle.Right,
                Width = 190,
                Height = 42,
                FlatStyle = FlatStyle.Flat,
                BackColor = primaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            openReportButton.FlatAppearance.BorderSize = 0;
            openReportButton.Click += btnLowStockReport_Click;

            reportCardLayout.Controls.Add(reportTitle, 0, 0);
            reportCardLayout.Controls.Add(reportDescription, 0, 1);
            reportCardLayout.Controls.Add(openReportButton, 0, 2);

            reportCard.Controls.Add(reportCardLayout);

            contentLayout.Controls.Add(dashboardTitle, 0, 0);
            contentLayout.Controls.Add(welcomeText, 0, 1);
            contentLayout.Controls.Add(productsCard, 0, 2);
            contentLayout.Controls.Add(reportCard, 0, 3);

            content.Controls.Add(contentLayout);

            mainLayout.Controls.Add(header, 0, 0);
            mainLayout.Controls.Add(content, 0, 1);

            Controls.Add(mainLayout);
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            using (var productsForm = new ProductsForm(
                _productService,
                _categoryService,
                _currentUser))
            {
                productsForm.ShowDialog(this);
            }
        }


        private void btnLowStockReport_Click(object sender, EventArgs e)
        {
            using (var reportForm = new LowStockReportForm(
                _productService,
                _categoryService))
            {
                reportForm.ShowDialog(this);
            }
        }
    }
}
