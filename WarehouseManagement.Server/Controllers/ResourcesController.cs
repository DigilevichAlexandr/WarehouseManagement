using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Server.Repositories;
using WarehouseManagement.Server.Services;
using WarehouseManagement.Shared.Models;
using WarehouseManagement.Shared.Enums;

namespace WarehouseManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourcesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWarehouseService _warehouseService;

        public ResourcesController(IUnitOfWork unitOfWork, IWarehouseService warehouseService)
        {
            _unitOfWork = unitOfWork;
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resource>>> GetResources()
        {
            var resources = await _unitOfWork.Resources.GetAllAsync();
            return Ok(resources);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Resource>>> GetActiveResources()
        {
            var resources = await _unitOfWork.Resources.FindAsync(r => r.State == EntityState.Active);
            return Ok(resources);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Resource>> GetResource(int id)
        {
            var resource = await _unitOfWork.Resources.GetByIdAsync(id);
            if (resource == null)
            {
                return NotFound();
            }
            return Ok(resource);
        }

        [HttpPost]
        public async Task<ActionResult<Resource>> CreateResource(Resource resource)
        {
            try
            {
                // Check for duplicate name
                var existingResource = await _unitOfWork.Resources
                    .ExistsAsync(r => r.Name == resource.Name);
                
                if (existingResource)
                {
                    return BadRequest($"Ресурс с наименованием '{resource.Name}' уже существует");
                }

                resource.CreatedAt = DateTime.UtcNow;
                resource.UpdatedAt = DateTime.UtcNow;
                resource.State = EntityState.Active;

                await _unitOfWork.Resources.AddAsync(resource);
                await _unitOfWork.SaveChangesAsync();

                return CreatedAtAction(nameof(GetResource), new { id = resource.Id }, resource);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResource(int id, Resource resource)
        {
            if (id != resource.Id)
            {
                return BadRequest();
            }

            try
            {
                // Check for duplicate name
                var existingResource = await _unitOfWork.Resources
                    .ExistsAsync(r => r.Name == resource.Name && r.Id != id);
                
                if (existingResource)
                {
                    return BadRequest($"Ресурс с наименованием '{resource.Name}' уже существует");
                }

                resource.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Resources.Update(resource);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResource(int id)
        {
            try
            {
                var resource = await _unitOfWork.Resources.GetByIdAsync(id);
                if (resource == null)
                {
                    return NotFound();
                }

                var canDelete = await _warehouseService.CanDeleteResourceAsync(id);
                if (!canDelete)
                {
                    return BadRequest("Нельзя удалить ресурс, который используется в документах или на балансе");
                }

                _unitOfWork.Resources.Remove(resource);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/archive")]
        public async Task<IActionResult> ArchiveResource(int id)
        {
            try
            {
                var resource = await _unitOfWork.Resources.GetByIdAsync(id);
                if (resource == null)
                {
                    return NotFound();
                }

                resource.State = EntityState.Archived;
                resource.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Resources.Update(resource);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreResource(int id)
        {
            try
            {
                var resource = await _unitOfWork.Resources.GetByIdAsync(id);
                if (resource == null)
                {
                    return NotFound();
                }

                resource.State = EntityState.Active;
                resource.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Resources.Update(resource);
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
