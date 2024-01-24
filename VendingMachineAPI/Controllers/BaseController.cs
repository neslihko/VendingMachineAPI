using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VendingMachineAPI.Controllers
{
    public class BaseController : Controller
    {
        public int CurrentBuyerId => int.TryParse(CurrentBuyerClaimId, out int userId) ? userId : 0;

        public string? CurrentBuyerClaimId =>
            User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}
