using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VendingMachineAPI.Dtos;
using VendingMachineAPI.Services;

namespace VendingMachineAPI.Controllers
{
    public class VendingMachineController : BaseController
    {
        private readonly IVendingMachineService _vendingMachineService;

        public VendingMachineController(IVendingMachineService vendingMachineService)
        {
            _vendingMachineService = vendingMachineService;
        }

        // Endpoint for depositing coins (POST /deposit)
        [HttpPost("deposit")]
        [Authorize(Roles = "buyer")]
        public IActionResult DepositCoins([FromBody] int coinValue)
        {
            try
            {
                // Call VendingMachineService to handle the deposit
                _vendingMachineService.DepositCoins(base.CurrentBuyerId, coinValue);

                // Return a success response
                return Ok("Deposit successful");
            }
            catch (ArgumentException ex)
            {
                // Handle validation or business logic errors
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                // Handle unauthorized access errors
                return Forbid();
            }
            catch (Exception)
            {
                // Handle other unexpected errors
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Endpoint for buying products (POST /buy)
        [HttpPost("buy")]
        [Authorize(Roles = "buyer")]
        public IActionResult BuyProduct([FromBody] BuyRequest buyRequest)
        {
            try
            {
                // Call VendingMachineService to handle the purchase
                var purchaseResult = _vendingMachineService.BuyProduct(base.CurrentBuyerId, buyRequest.ProductId, buyRequest.Amount);

                // Return the purchase result in the response
                return Ok(purchaseResult);
            }
            catch (ArgumentException ex)
            {
                // Handle validation or business logic errors
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                // Handle unauthorized access errors
                return Forbid();
            }
            catch (Exception)
            {
                // Handle other unexpected errors
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Endpoint for resetting deposit (POST /reset)
        [HttpPost("reset")]
        [Authorize(Roles = "buyer")]
        public IActionResult ResetDeposit()
        {
            try
            {
                // Call VendingMachineService to handle the deposit reset
                _vendingMachineService.ResetDeposit(base.CurrentBuyerId);

                // Return a success response
                return Ok("Deposit reset successful");
            }
            catch (ArgumentException ex)
            {
                // Handle validation or business logic errors
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                // Handle unauthorized access errors
                return Forbid();
            }
            catch (Exception)
            {
                // Handle other unexpected errors
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
