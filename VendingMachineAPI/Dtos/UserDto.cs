namespace VendingMachineAPI.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public decimal Deposit { get; set; }
        public string Role { get; set; }
        public string SessionId { get; set; } // For session management
    }
}