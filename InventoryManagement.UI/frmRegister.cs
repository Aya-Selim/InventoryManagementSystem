
using System;
using System.Drawing;
using System.Windows.Forms;
using InventoryManagement.Service.Services;

namespace InventoryManagement.UI
{
    public partial class frmRegister : Form
    {
        private readonly UserService _userService;

        public frmRegister(UserService userService)
        {
            InitializeComponent();

            _userService = userService
                ?? throw new ArgumentNullException(nameof(userService));

            ConfigureLayout();

            btnRegister.Click += btnRegister_Click;
            btnCancel.Click += btnCancel_Click;
        }

        private void ConfigureLayout()
        {
            Text = "Create Account";
            StartPosition = FormStartPosition.CenterParent;

            MinimumSize = new Size(460, 620);
            Size = new Size(520, 700);

            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 10F);

            txtFirstName.MaxLength = 50;
            txtLastName.MaxLength = 50;
            txtEmail.MaxLength = 256;

            txtPassword.UseSystemPasswordChar = true;
            txtConfirmPassword.UseSystemPasswordChar = true;

            lblFirstName.Text = "First Name";
            lblLastName.Text = "Last Name";
            lblEmail.Text = "Email Address";
            lblPassword.Text = "Password";
            lblConfirmPassword.Text = "Confirm Password";

            lblError.Text = "";
            lblError.ForeColor = Color.Firebrick;

            btnRegister.Text = "Create Account";
            btnCancel.Text = "Cancel";

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                ColumnCount = 1,
                RowCount = 12,
                AutoScroll = true
            };

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            //layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            layout.Controls.Add(lblFirstName, 0, 0);
            layout.Controls.Add(txtFirstName, 0, 1);
            layout.Controls.Add(lblLastName, 0, 2);
            layout.Controls.Add(txtLastName, 0, 3);
            layout.Controls.Add(lblEmail, 0, 4);
            layout.Controls.Add(txtEmail, 0, 5);
            layout.Controls.Add(lblPassword, 0, 6);
            layout.Controls.Add(txtPassword, 0, 7);
            layout.Controls.Add(lblConfirmPassword, 0, 8);
            layout.Controls.Add(txtConfirmPassword, 0, 9);
            layout.Controls.Add(lblError, 0, 10);
            //layout.Controls.Add(btnRegister, 0, 11);
            //layout.Controls.Add(btnCancel, 0, 12);
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                AutoSize = false,
                Padding = new Padding(0, 3, 0, 3)
            };

            btnRegister.Size = new Size(150, 38);
            btnCancel.Size = new Size(100, 38);

            btnRegister.Margin = new Padding(5, 0, 0, 0);
            btnCancel.Margin = new Padding(5, 0, 0, 0);

            buttonsPanel.Controls.Add(btnCancel);
            buttonsPanel.Controls.Add(btnRegister);

            layout.Controls.Add(buttonsPanel, 0, 11);

            foreach (Control control in new Control[]
            {
                lblFirstName, lblLastName, lblEmail,
                lblPassword, lblConfirmPassword, lblError
            })
            {
                control.Dock = DockStyle.Fill;
                control.Margin = new Padding(3, 2, 3, 2);
            }

            foreach (Control control in new Control[]
            {
                txtFirstName, txtLastName, txtEmail,
                txtPassword, txtConfirmPassword
            })
            {
                control.Dock = DockStyle.Fill;
                control.Margin = new Padding(3, 2, 3, 5);
            }

            btnRegister.Dock = DockStyle.None;
            btnCancel.Dock = DockStyle.None;

            btnRegister.Size = new Size(150, 38);
            btnCancel.Size = new Size(100, 38);

            btnRegister.Margin = new Padding(5, 0, 0, 0);
            btnCancel.Margin = new Padding(5, 0, 0, 0);

            Controls.Clear();
            Controls.Add(layout);

            AcceptButton = btnRegister;
            CancelButton = btnCancel;
        }


        private void btnRegister_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            btnRegister.Enabled = false;

            try
            {
                var result = _userService.Register(
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

                MessageBox.Show(
                    "Account created successfully. You can now log in.",
                    "Registration Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception)
            {
                lblError.Text =
                    "Unable to create the account. Please try again.";
            }
            finally
            {
                btnRegister.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
