using InventoryManagement.Service.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryManagement.UI
{
    public partial class frmSetup : Form
    {
        private readonly UserService _userService;
        public frmSetup(UserService userService)
        {
            InitializeComponent();

            _userService = userService
                ?? throw new ArgumentNullException(nameof(userService));

            ConfigureLayout();
        }


        private void ConfigureLayout()
        {
            // Form settings
            Text = "Initial Setup";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(520, 520);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Labels
            lblFirstName.Text = "First name";
            lblLastName.Text = "Last name";
            lblEmail.Text = "Email";
            lblPassword.Text = "Password";
            lblConfirmPassword.Text = "Confirm password";

            lblError.Text = "";
            lblError.ForeColor = Color.Red;
            lblError.AutoSize = false;
            lblError.TextAlign = ContentAlignment.MiddleLeft;

            // TextBoxes
            txtPassword.UseSystemPasswordChar = true;
            txtConfirmPassword.UseSystemPasswordChar = true;

            txtFirstName.MaxLength = 50;
            txtLastName.MaxLength = 50;
            txtEmail.MaxLength = 256;

            // Button
            btnCreate.Text = "Create admin account";
            btnCreate.Click += btnCreate_Click;
            AcceptButton = btnCreate;

            // Tab order
            txtFirstName.TabIndex = 0;
            txtLastName.TabIndex = 1;
            txtEmail.TabIndex = 2;
            txtPassword.TabIndex = 3;
            txtConfirmPassword.TabIndex = 4;
            btnCreate.TabIndex = 5;

            // Create layout
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                ColumnCount = 2,
                RowCount = 10,
                AutoSize = false
            };

            // Columns
            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50F));

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50F));

            // Rows
            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 25F));  // 0: Name labels

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 35F));  // 1: Name inputs

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 25F));  // 2: Email label

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 35F));  // 3: Email input

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 25F));  // 4: Password label

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 35F));  // 5: Password input

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 25F));  // 6: Confirm label

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 35F));  // 7: Confirm input

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 45F));  // 8: Error message

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));  // 9: Button
            layout.RowStyles[9] =
                new RowStyle(SizeType.Absolute, 45F);
            // First name and Last name
            layout.Controls.Add(lblFirstName, 0, 0);
            layout.Controls.Add(lblLastName, 1, 0);

            layout.Controls.Add(txtFirstName, 0, 1);
            layout.Controls.Add(txtLastName, 1, 1);

            // Email
            layout.Controls.Add(lblEmail, 0, 2);
            layout.SetColumnSpan(lblEmail, 2);

            layout.Controls.Add(txtEmail, 0, 3);
            layout.SetColumnSpan(txtEmail, 2);

            // Password
            layout.Controls.Add(lblPassword, 0, 4);
            layout.SetColumnSpan(lblPassword, 2);

            layout.Controls.Add(txtPassword, 0, 5);
            layout.SetColumnSpan(txtPassword, 2);

            // Confirm password
            layout.Controls.Add(lblConfirmPassword, 0, 6);
            layout.SetColumnSpan(lblConfirmPassword, 2);

            layout.Controls.Add(txtConfirmPassword, 0, 7);
            layout.SetColumnSpan(txtConfirmPassword, 2);

            // Error message
            layout.Controls.Add(lblError, 0, 8);
            layout.SetColumnSpan(lblError, 2);

            // Create button
            layout.Controls.Add(btnCreate, 0, 9);
            layout.SetColumnSpan(btnCreate, 2);


            foreach (Control control in layout.Controls)
            {
                control.Margin = new Padding(3);

                if (control is TextBox)
                {
                    control.Dock = DockStyle.Fill;
                }
                else if (control is Label label)
                {
                    label.Dock = DockStyle.Fill;
                    label.TextAlign = ContentAlignment.MiddleLeft;
                }
                else if (control is Button)
                {
                    control.Dock = DockStyle.None;
                    control.Anchor = AnchorStyles.None;
                    control.Size = new Size(240, 38);
                }
            }
            // Add layout to form
            Controls.Clear();
            Controls.Add(layout);
        }

        private void btnCreate_Click(object sender, EventArgs e)    
        {
            var result = _userService.SetupAdmin(
                txtFirstName.Text,
                txtLastName.Text,
                txtEmail.Text,
                txtPassword.Text,
                txtConfirmPassword.Text);

            if (!result.IsSuccess)
            {
                lblError.Text = result.Message;
                return;
            }

            //MessageBox.Show(
            //    "Admin account created successfully.",
            //    "Success",
            //    MessageBoxButtons.OK,
            //    MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;

        }

    }
}
