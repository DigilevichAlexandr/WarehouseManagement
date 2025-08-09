using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Server.Services;
using WarehouseManagement.Shared.Models;
using WarehouseManagement.Shared.DTOs;

namespace WarehouseManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentDocumentsController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public ShipmentDocumentsController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipmentDocument>>> GetShipmentDocuments([FromQuery] DocumentFilterDto? filter = null)
        {
            try
            {
                var documents = await _warehouseService.GetShipmentDocumentsAsync(filter);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShipmentDocument>> GetShipmentDocument(int id)
        {
            try
            {
                var document = await _warehouseService.GetShipmentDocumentByIdAsync(id);
                if (document == null)
                {
                    return NotFound();
                }
                return Ok(document);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ShipmentDocument>> CreateShipmentDocument(ShipmentDocument document)
        {
            try
            {
                var createdDocument = await _warehouseService.CreateShipmentDocumentAsync(document);
                return CreatedAtAction(nameof(GetShipmentDocument), new { id = createdDocument.Id }, createdDocument);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipmentDocument(int id, ShipmentDocument document)
        {
            if (id != document.Id)
            {
                return BadRequest();
            }

            try
            {
                await _warehouseService.UpdateShipmentDocumentAsync(document);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShipmentDocument(int id)
        {
            try
            {
                await _warehouseService.DeleteShipmentDocumentAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/sign")]
        public async Task<ActionResult<ShipmentDocument>> SignShipmentDocument(int id)
        {
            try
            {
                var document = await _warehouseService.SignShipmentDocumentAsync(id);
                return Ok(document);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/revoke")]
        public async Task<ActionResult<ShipmentDocument>> RevokeShipmentDocument(int id)
        {
            try
            {
                var document = await _warehouseService.RevokeShipmentDocumentAsync(id);
                return Ok(document);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
