namespace VendingMachineAPI.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        public decimal Deposit { get; set; }
        public string Role { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string SessionId { get; set; } // For session management
    }
}
