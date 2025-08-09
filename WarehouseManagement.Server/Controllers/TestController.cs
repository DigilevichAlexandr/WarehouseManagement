using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace WarehouseManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly WarehouseContext _context;

        public TestController(WarehouseContext context)
        {
            _context = context;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            try
            {
                return Ok(new { 
                    status = "healthy", 
                    timestamp = DateTime.UtcNow,
                    message = "API is working correctly"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("database")]
        public async Task<IActionResult> DatabaseTest()
        {
            try
            {
                // Test database connection
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    return StatusCode(500, new { error = "Cannot connect to database" });
                }

                // Get table counts
                var resourcesCount = await _context.Resources.CountAsync();
                var unitsCount = await _context.UnitsOfMeasurement.CountAsync();
                var clientsCount = await _context.Clients.CountAsync();
                var balanceCount = await _context.Balances.CountAsync();
                var receiptDocsCount = await _context.ReceiptDocuments.CountAsync();
                var shipmentDocsCount = await _context.ShipmentDocuments.CountAsync();

                return Ok(new
                {
                    status = "database_healthy",
                    timestamp = DateTime.UtcNow,
                    connection = "successful",
                    tables = new
                    {
                        resources = resourcesCount,
                        unitsOfMeasurement = unitsCount,
                        clients = clientsCount,
                        balance = balanceCount,
                        receiptDocuments = receiptDocsCount,
                        shipmentDocuments = shipmentDocsCount
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database test error: {ex}");
                return StatusCode(500, new { 
                    error = ex.Message, 
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("documents/receipt")]
        public async Task<IActionResult> TestReceiptDocuments()
        {
            try
            {
                // Simple query without includes
                var simpleCount = await _context.ReceiptDocuments.CountAsync();
                var simpleList = await _context.ReceiptDocuments.ToListAsync();
                
                // Query with includes
                var docsWithIncludes = await _context.ReceiptDocuments
                    .Include(d => d.ReceiptResources)
                        .ThenInclude(r => r.Resource)
                    .Include(d => d.ReceiptResources)
                        .ThenInclude(r => r.UnitOfMeasurement)
                    .ToListAsync();

                return Ok(new
                {
                    status = "receipt_documents_test",
                    simpleCount = simpleCount,
                    withIncludesCount = docsWithIncludes.Count,
                    simpleDocuments = simpleList,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Receipt documents test error: {ex}");
                return StatusCode(500, new { 
                    error = ex.Message, 
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("documents/receipt/simple")]
        public async Task<IActionResult> TestReceiptDocumentsSimple()
        {
            try
            {
                var documents = await _context.ReceiptDocuments.ToListAsync();
                return Ok(documents);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Simple receipt documents error: {ex}");
                return StatusCode(500, new { 
                    error = ex.Message, 
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("documents/shipment")]
        public async Task<IActionResult> TestShipmentDocuments()
        {
            try
            {
                // Simple query without includes
                var simpleCount = await _context.ShipmentDocuments.CountAsync();
                
                // Query with includes
                var docsWithIncludes = await _context.ShipmentDocuments
                    .Include(d => d.Client)
                    .Include(d => d.ShipmentResources)
                        .ThenInclude(r => r.Resource)
                    .Include(d => d.ShipmentResources)
                        .ThenInclude(r => r.UnitOfMeasurement)
                    .ToListAsync();

                return Ok(new
                {
                    status = "shipment_documents_test",
                    simpleCount = simpleCount,
                    withIncludesCount = docsWithIncludes.Count,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Shipment documents test error: {ex}");
                return StatusCode(500, new { 
                    error = ex.Message, 
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }
    }
}
