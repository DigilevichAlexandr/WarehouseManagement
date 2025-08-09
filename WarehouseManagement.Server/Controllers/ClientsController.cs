using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Server.Repositories;
using WarehouseManagement.Server.Services;
using WarehouseManagement.Shared.Models;
using WarehouseManagement.Shared.Enums;

namespace WarehouseManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWarehouseService _warehouseService;

        public ClientsController(IUnitOfWork unitOfWork, IWarehouseService warehouseService)
        {
            _unitOfWork = unitOfWork;
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            var clients = await _unitOfWork.Clients.GetAllAsync();
            return Ok(clients);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Client>>> GetActiveClients()
        {
            var clients = await _unitOfWork.Clients.FindAsync(c => c.State == EntityState.Active);
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }

        [HttpPost]
        public async Task<ActionResult<Client>> CreateClient(Client client)
        {
            try
            {
                // Check for duplicate name
                var existingClient = await _unitOfWork.Clients
                    .ExistsAsync(c => c.Name == client.Name);
                
                if (existingClient)
                {
                    return BadRequest($"Клиент с наименованием '{client.Name}' уже существует");
                }

                client.CreatedAt = DateTime.UtcNow;
                client.UpdatedAt = DateTime.UtcNow;
                client.State = EntityState.Active;

                await _unitOfWork.Clients.AddAsync(client);
                await _unitOfWork.SaveChangesAsync();

                return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, Client client)
        {
            if (id != client.Id)
            {
                return BadRequest();
            }

            try
            {
                // Check for duplicate name
                var existingClient = await _unitOfWork.Clients
                    .ExistsAsync(c => c.Name == client.Name && c.Id != id);
                
                if (existingClient)
                {
                    return BadRequest($"Клиент с наименованием '{client.Name}' уже существует");
                }

                client.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Clients.Update(client);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            try
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(id);
                if (client == null)
                {
                    return NotFound();
                }

                var canDelete = await _warehouseService.CanDeleteClientAsync(id);
                if (!canDelete)
                {
                    return BadRequest("Нельзя удалить клиента, который используется в документах отгрузки");
                }

                _unitOfWork.Clients.Remove(client);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/archive")]
        public async Task<IActionResult> ArchiveClient(int id)
        {
            try
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(id);
                if (client == null)
                {
                    return NotFound();
                }

                client.State = EntityState.Archived;
                client.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Clients.Update(client);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreClient(int id)
        {
            try
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(id);
                if (client == null)
                {
                    return NotFound();
                }

                client.State = EntityState.Active;
                client.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Clients.Update(client);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
