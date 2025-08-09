using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Server.Services;
using WarehouseManagement.Shared.Models;
using WarehouseManagement.Shared.DTOs;

namespace WarehouseManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptDocumentsController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public ReceiptDocumentsController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet("test")]
        public IActionResult TestEndpoint()
        {
            return Ok(new { message = "Receipt documents API is working", timestamp = DateTime.UtcNow });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceiptDocument>>> GetReceiptDocuments([FromQuery] DocumentFilterDto? filter = null)
        {
            try
            {
                var documents = await _warehouseService.GetReceiptDocumentsAsync(filter);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error in GetReceiptDocuments: {ex}");
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReceiptDocument>> GetReceiptDocument(int id)
        {
            try
            {
                var document = await _warehouseService.GetReceiptDocumentByIdAsync(id);
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
        public async Task<ActionResult<ReceiptDocument>> CreateReceiptDocument(ReceiptDocument document)
        {
            try
            {
                var createdDocument = await _warehouseService.CreateReceiptDocumentAsync(document);
                return CreatedAtAction(nameof(GetReceiptDocument), new { id = createdDocument.Id }, createdDocument);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReceiptDocument(int id, ReceiptDocument document)
        {
            if (id != document.Id)
            {
                return BadRequest();
            }

            try
            {
                await _warehouseService.UpdateReceiptDocumentAsync(document);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceiptDocument(int id)
        {
            try
            {
                await _warehouseService.DeleteReceiptDocumentAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
