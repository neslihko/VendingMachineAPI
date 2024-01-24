namespace VendingMachineAPI.Services
{
    using System.Collections.Generic;
    using VendingMachineAPI.Helpers;
    using VendingMachineAPI.Model;
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public User? Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            // Check if username exists
            if (user == null)
            {
                return null;
            }

            // Check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        public IEnumerable<User> GetAll() => _context.Users;

        public User? GetById(int id) => _context.Users.Find(id);

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AppException("Password is required");
            }

            if (_context.Users.Any(x => x.Username == user.Username))
            {
                throw new AppException($"Username {user.Username} taken");
            }

            HashUtility.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public void Update(User user, string? password = null)
        {
            var existing = _context.Users.Find(user.UserId);

            if (existing == null)
            {
                throw new AppException("User not found");
            }

            if (user.Username != existing.Username && _context.Users.Any(x => x.Username == user.Username))
            {
                throw new AppException($"Username {existing.Username} taken");
            }

            existing.Deposit = user.Deposit;
            existing.Role = user.Role;
            existing.Username = user.Username;

            // Update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                HashUtility.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

                existing.PasswordHash = passwordHash;
                existing.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(existing);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            ArgumentException.ThrowIfNullOrEmpty(password);

            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i])
                {
                    return false;
                }
            }

            return true;
        }

        public decimal DepositCoin(int userId, int coin)
        {
            var user = GetById(userId);

            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            // Check if the user is a buyer
            if (user.Role.ToLower() != "buyer")
            {
                throw new InvalidOperationException("Only buyers can deposit coins.");
            }

            // Validate coin denomination
            if (!IsValidCoinDenomination(coin))
            {
                throw new ArgumentException($"Invalid coin denomination. Allowed denominations: {string.Join(", ", Constants.AllowedDenominations)}.");
            }

            // Update user's deposit
            user.Deposit += coin;

            _context.Users.Update(user);
            _context.SaveChanges();

            return user.Deposit;
        }

        private bool IsValidCoinDenomination(int coin) => Constants.AllowedDenominations.Contains(coin);
    }
}
