using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Diagnostics;
using InventoryManagement.Service.Security;

namespace InventoryManagement.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Pbkdf2PasswordHasher hasher = new Pbkdf2PasswordHasher();

            string password = "MyPass123";

            // 1. Measure hashing time
            Stopwatch stopwatch = Stopwatch.StartNew();

            var result1 = hasher.HashPassword(password);

            stopwatch.Stop();
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            Stopwatch verifyStopwatch = Stopwatch.StartNew();

            bool correctPassword = hasher.VerifyPassword(
                password, result1.Hash, result1.Salt);

            verifyStopwatch.Stop();

            long verifyMilliseconds =
                verifyStopwatch.ElapsedMilliseconds;

          

            bool wrongPassword = hasher.VerifyPassword(
                "WrongPass123", result1.Hash, result1.Salt);

            // 3. Hash the same password again
            var result2 = hasher.HashPassword(password);

            // 4. Compare hashes and salts
            bool sameHash =
                Convert.ToBase64String(result1.Hash) ==
                Convert.ToBase64String(result2.Hash);

            bool sameSalt =
                Convert.ToBase64String(result1.Salt) ==
                Convert.ToBase64String(result2.Salt);

            // 5. Display all results in one message
            string message =
                "Correct password: " + correctPassword +
                "\nWrong password: " + wrongPassword +
                "\nSame hash: " + sameHash +
                "\nSame salt: " + sameSalt +
                "\nHashing time: " + elapsedMilliseconds + " ms";

            MessageBox.Show(message, "Password Hasher Test");
        }


    }
}
