
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using InventoryManagement.Core.Entities;
using InventoryManagement.Service.Common;
using InventoryManagement.Service.Services;

namespace InventoryManagement.UI
{
    public partial class frmLogin : Form
    {
        private readonly UserService _userService;

        public User LoggedInUser { get; private set; }

        public frmLogin(UserService userService)
        {
            InitializeComponent();

            _userService = userService
                ?? throw new ArgumentNullException(nameof(userService));

            ConfigureLayout();
        }

        private void ConfigureLayout()
        {
            Text = "Inventory Management - Login";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(480, 420);
            MinimumSize = new Size(480, 420);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            lblEmail.Text = "Email";
            lblPassword.Text = "Password";
            lblError.Text = "";
            lblError.ForeColor = Color.Firebrick;

            btnLogin.Text = "Login";
            btnRegister.Text = "Create Account";

            txtEmail.MaxLength = 256;
            txtEmail.TabIndex = 0;

            txtPassword.UseSystemPasswordChar = true;
            txtPassword.TabIndex = 1;

            btnLogin.TabIndex = 2;
            btnLogin.Size = new Size(160, 38);
            btnLogin.Anchor = AnchorStyles.None;

            btnRegister.TabIndex = 3;
            btnRegister.Size = new Size(160, 38);
            btnRegister.Anchor = AnchorStyles.None;

            AcceptButton = btnLogin;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                ColumnCount = 1,
                RowCount = 7
            };

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 35));
            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 35));
            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 45));
            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 45));

            layout.Controls.Add(lblEmail, 0, 0);
            layout.Controls.Add(txtEmail, 0, 1);
            layout.Controls.Add(lblPassword, 0, 2);
            layout.Controls.Add(txtPassword, 0, 3);
            layout.Controls.Add(lblError, 0, 4);
            layout.Controls.Add(btnLogin, 0, 5);
            layout.Controls.Add(btnRegister, 0, 6);

            foreach (Control control in new Control[]
            {
                lblEmail, txtEmail,
                lblPassword, txtPassword,
                lblError, btnLogin, btnRegister
            })
            {
                control.Margin = new Padding(3);
            }

            txtEmail.Dock = DockStyle.Fill;
            txtPassword.Dock = DockStyle.Fill;

            foreach (Label label in new Label[]
            {
                lblEmail, lblPassword, lblError
            })
            {
                label.Dock = DockStyle.Fill;
                label.TextAlign = ContentAlignment.MiddleLeft;
            }

            Controls.Clear();
            Controls.Add(layout);

            btnLogin.Click += btnLogin_Click;
            btnRegister.Click += btnRegister_Click;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            string email = txtEmail.Text;
            string password = txtPassword.Text;

            btnLogin.Enabled = false;
            btnLogin.Text = "Logging in...";

            try
            {
                ServiceResult<User> result = await Task.Run(
                    () => _userService.Login(email, password));

                if (!result.IsSuccess)
                {
                    lblError.Text = result.Message;
                    return;
                }

                LoggedInUser = result.Data;
                DialogResult = DialogResult.OK;
            }
            catch (Exception)
            {
                lblError.Text =
                    "Unable to log in. Please try again.";
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (var form = new frmRegister(_userService))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    txtPassword.Clear();
                    txtEmail.Focus();
                    lblError.Text = "";
                }
            }
        }
    }
}
