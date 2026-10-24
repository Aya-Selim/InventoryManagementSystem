using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Core.Models;
using InventoryManagement.Core.Enums;
using InventoryManagement.Service.Common;
using System;
using System.Linq;

namespace InventoryManagement.Service.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));

            _passwordHasher = passwordHasher
                ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public bool NeedsAdminSetup()
        {
            return !_userRepository.HasAnyUsers();
        }

        public ServiceResult SetupAdmin(
              string firstName,
              string lastName,
              string email,
              string password,
              string confirmPassword)
        {
            if (_userRepository.HasAnyUsers())
            {
                return ServiceResult.Fail(
                    "System setup has already been completed.");
            }

            return CreateUser(
                firstName,
                lastName,
                email,
                password,
                confirmPassword,
                UserRole.Admin);
        }

        public ServiceResult Register(
               string firstName,
               string lastName,
               string email,
               string password,
               string confirmPassword)
        {
            return CreateUser(
                firstName,
                lastName,
                email,
                password,
                confirmPassword,
                UserRole.Viewer);
        }
        public ServiceResult<User> Login(
              string email,
              string password)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return ServiceResult<User>.Fail(
                    "Invalid email or password.");
            }

            email = email.Trim().ToLowerInvariant();

            User user = _userRepository.GetByEmail(email);

            if (user == null ||
                !user.IsActive ||
                !_passwordHasher.VerifyPassword(
                    password,
                    user.PasswordHash,
                    user.PasswordSalt))
            {
                return ServiceResult<User>.Fail(
                    "Invalid email or password.");
            }

            // Do not expose password credentials to the caller.
            user.PasswordHash = null;
            user.PasswordSalt = null;

            return ServiceResult<User>.Ok(user);
        }
        private ServiceResult CreateUser(
            string firstName,
            string lastName,
            string email,
            string password,
            string confirmPassword,
            UserRole role)
        {
            firstName = firstName?.Trim();
            lastName = lastName?.Trim();
            email = email?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                return ServiceResult.Fail("All fields are required.");
            }

            if (firstName.Length > 50 || lastName.Length > 50)
            {
                return ServiceResult.Fail(
                    "Name cannot exceed 50 characters.");
            }

            if (email.Length > 256)
            {
                return ServiceResult.Fail(
                    "Email cannot exceed 256 characters.");
            }

            try
            {
                var address = new System.Net.Mail.MailAddress(email);

                if (address.Address != email)
                {
                    return ServiceResult.Fail("Invalid email format.");
                }
            }
            catch (FormatException)
            {
                return ServiceResult.Fail("Invalid email format.");
            }

            if (password.Length < 12 ||
                !password.Any(char.IsUpper) ||
                !password.Any(char.IsLower) ||
                !password.Any(char.IsDigit) ||
                !password.Any(c => !char.IsLetterOrDigit(c)))
            {
                return ServiceResult.Fail(
                    "Password does not meet the password policy,At least 12 characters with upper, lower, digit and symbol");
            }

            if (password != confirmPassword)
            {
                return ServiceResult.Fail("Passwords do not match.");
            }

            if (_userRepository.EmailExists(email))
            {
                return ServiceResult.Fail(
                    "This email is already registered.");
            }

            PasswordHashResult passwordResult =
                _passwordHasher.HashPassword(password);

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = passwordResult.Hash,
                PasswordSalt = passwordResult.Salt,
                Role = role,
                IsActive = true
            };

            _userRepository.Add(user);

            return ServiceResult.Ok();
        }
    }
}