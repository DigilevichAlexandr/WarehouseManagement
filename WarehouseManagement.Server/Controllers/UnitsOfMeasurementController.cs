using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Server.Repositories;
using WarehouseManagement.Server.Services;
using WarehouseManagement.Shared.Models;
using WarehouseManagement.Shared.Enums;

namespace WarehouseManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsOfMeasurementController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWarehouseService _warehouseService;

        public UnitsOfMeasurementController(IUnitOfWork unitOfWork, IWarehouseService warehouseService)
        {
            _unitOfWork = unitOfWork;
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitOfMeasurement>>> GetUnitsOfMeasurement()
        {
            var units = await _unitOfWork.UnitsOfMeasurement.GetAllAsync();
            return Ok(units);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<UnitOfMeasurement>>> GetActiveUnitsOfMeasurement()
        {
            var units = await _unitOfWork.UnitsOfMeasurement.FindAsync(u => u.State == EntityState.Active);
            return Ok(units);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UnitOfMeasurement>> GetUnitOfMeasurement(int id)
        {
            var unit = await _unitOfWork.UnitsOfMeasurement.GetByIdAsync(id);
            if (unit == null)
            {
                return NotFound();
            }
            return Ok(unit);
        }

        [HttpPost]
        public async Task<ActionResult<UnitOfMeasurement>> CreateUnitOfMeasurement(UnitOfMeasurement unit)
        {
            try
            {
                // Check for duplicate name
                var existingUnit = await _unitOfWork.UnitsOfMeasurement
                    .ExistsAsync(u => u.Name == unit.Name);
                
                if (existingUnit)
                {
                    return BadRequest($"Единица измерения с наименованием '{unit.Name}' уже существует");
                }

                unit.CreatedAt = DateTime.UtcNow;
                unit.UpdatedAt = DateTime.UtcNow;
                unit.State = EntityState.Active;

                await _unitOfWork.UnitsOfMeasurement.AddAsync(unit);
                await _unitOfWork.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUnitOfMeasurement), new { id = unit.Id }, unit);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnitOfMeasurement(int id, UnitOfMeasurement unit)
        {
            if (id != unit.Id)
            {
                return BadRequest();
            }

            try
            {
                // Check for duplicate name
                var existingUnit = await _unitOfWork.UnitsOfMeasurement
                    .ExistsAsync(u => u.Name == unit.Name && u.Id != id);
                
                if (existingUnit)
                {
                    return BadRequest($"Единица измерения с наименованием '{unit.Name}' уже существует");
                }

                unit.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.UnitsOfMeasurement.Update(unit);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnitOfMeasurement(int id)
        {
            try
            {
                var unit = await _unitOfWork.UnitsOfMeasurement.GetByIdAsync(id);
                if (unit == null)
                {
                    return NotFound();
                }

                var canDelete = await _warehouseService.CanDeleteUnitOfMeasurementAsync(id);
                if (!canDelete)
                {
                    return BadRequest("Нельзя удалить единицу измерения, которая используется в документах или на балансе");
                }

                _unitOfWork.UnitsOfMeasurement.Remove(unit);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/archive")]
        public async Task<IActionResult> ArchiveUnitOfMeasurement(int id)
        {
            try
            {
                var unit = await _unitOfWork.UnitsOfMeasurement.GetByIdAsync(id);
                if (unit == null)
                {
                    return NotFound();
                }

                unit.State = EntityState.Archived;
                unit.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.UnitsOfMeasurement.Update(unit);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreUnitOfMeasurement(int id)
        {
            try
            {
                var unit = await _unitOfWork.UnitsOfMeasurement.GetByIdAsync(id);
                if (unit == null)
                {
                    return NotFound();
                }

                unit.State = EntityState.Active;
                unit.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.UnitsOfMeasurement.Update(unit);
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
