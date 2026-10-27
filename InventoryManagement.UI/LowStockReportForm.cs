using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventoryManagement.Service.Services;

namespace InventoryManagement.UI
{
    public partial class LowStockReportForm : Form
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public LowStockReportForm(
            ProductService productService,
            CategoryService categoryService)
        {
            InitializeComponent();

            _productService = productService
                ?? throw new ArgumentNullException(nameof(productService));

            _categoryService = categoryService
                ?? throw new ArgumentNullException(nameof(categoryService));

            try
            {
                LoadReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Crystal Reports Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadReport()
        {
            var dataBuilder = new LowStockReportDataBuilder(
                _productService,
                _categoryService);

            DataTable reportData = dataBuilder.Build();

            var report = new LowStockReport();
            report.SetDataSource(reportData);

            crystalReportViewer1.ReportSource = report;
        }
    }
}
