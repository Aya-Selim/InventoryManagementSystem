using InventoryManagement.Core.Entities;
using InventoryManagement.Data;
using InventoryManagement.Data.Repositories;
using InventoryManagement.Service.Security;
using InventoryManagement.Service.Services;
using System;
using System.Configuration;
using System.Windows.Forms;


namespace InventoryManagement.UI
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                string connectionString =
                    ConfigurationManager
                        .ConnectionStrings["InventoryDb"]
                        .ConnectionString;

                var userRepository = new UserRepository(connectionString);
                var passwordHasher = new Pbkdf2PasswordHasher();

                var unitOfWork = new SqlUnitOfWork(connectionString);
                var categoryService = new CategoryService(unitOfWork);
                var productService = new ProductService(
                    unitOfWork,
                    userRepository);
                var userService = new UserService(
                    userRepository,
                    passwordHasher);

                if (userService.NeedsAdminSetup())
                {
                    using (var setupForm = new frmSetup(userService))
                    {
                        if (setupForm.ShowDialog() != DialogResult.OK)
                            return;
                    }
                }

                User loggedInUser;

                using (var loginForm = new frmLogin(userService))
                {
                    if (loginForm.ShowDialog() != DialogResult.OK)
                        return;

                    loggedInUser = loginForm.LoggedInUser;
                }

                Application.Run(new frmMain(
                     loggedInUser,
                     productService,
                     categoryService));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to start the application.\n\n" + ex.Message,
                    "Startup Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
