namespace VendingMachineAPI.Services
{
    public static class Constants
    {
        public static readonly IReadOnlyList<int> AllowedDenominations = new List<int> { 5, 10, 20, 50, 100 };
    }
}
