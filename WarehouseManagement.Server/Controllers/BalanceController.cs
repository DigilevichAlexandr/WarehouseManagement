using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Server.Services;
using WarehouseManagement.Shared.Models;
using WarehouseManagement.Shared.DTOs;

namespace WarehouseManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public BalanceController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Balance>>> GetBalance([FromQuery] BalanceFilterDto? filter = null)
        {
            try
            {
                var balance = await _warehouseService.GetBalanceAsync(filter);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
