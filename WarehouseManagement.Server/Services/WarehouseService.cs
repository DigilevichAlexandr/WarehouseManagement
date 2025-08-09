using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Server.Data;
using WarehouseManagement.Server.Repositories;
using WarehouseManagement.Shared.Models;
using WarehouseManagement.Shared.DTOs;
using WarehouseManagement.Shared.Enums;

namespace WarehouseManagement.Server.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly WarehouseContext _context;

        public WarehouseService(IUnitOfWork unitOfWork, WarehouseContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IEnumerable<Balance>> GetBalanceAsync(BalanceFilterDto? filter = null)
        {
            var query = _context.Balances
                .Include(b => b.Resource)
                .Include(b => b.UnitOfMeasurement)
                .AsQueryable();

            if (filter != null)
            {
                if (filter.ResourceIds.Any())
                {
                    query = query.Where(b => filter.ResourceIds.Contains(b.ResourceId));
                }
                
                if (filter.UnitOfMeasurementIds.Any())
                {
                    query = query.Where(b => filter.UnitOfMeasurementIds.Contains(b.UnitOfMeasurementId));
                }
            }

            return await query.ToListAsync();
        }

        public async Task UpdateBalanceAsync(int resourceId, int unitOfMeasurementId, decimal quantityChange)
        {
            var balance = await _context.Balances
                .FirstOrDefaultAsync(b => b.ResourceId == resourceId && b.UnitOfMeasurementId == unitOfMeasurementId);

            if (balance == null)
            {
                if (quantityChange > 0)
                {
                    balance = new Balance
                    {
                        ResourceId = resourceId,
                        UnitOfMeasurementId = unitOfMeasurementId,
                        Quantity = quantityChange,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Balances.AddAsync(balance);
                }
            }
            else
            {
                balance.Quantity += quantityChange;
                balance.UpdatedAt = DateTime.UtcNow;
                
                if (balance.Quantity < 0)
                {
                    throw new InvalidOperationException("Недостаточно ресурсов на складе");
                }
                
                if (balance.Quantity == 0)
                {
                    _unitOfWork.Balances.Remove(balance);
                }
                else
                {
                    _unitOfWork.Balances.Update(balance);
                }
            }
        }

        public async Task<ReceiptDocument> CreateReceiptDocumentAsync(ReceiptDocument document)
        {
            await ValidateReceiptDocumentAsync(document);
            
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                document.CreatedAt = DateTime.UtcNow;
                document.UpdatedAt = DateTime.UtcNow;
                
                await _unitOfWork.ReceiptDocuments.AddAsync(document);
                await _unitOfWork.SaveChangesAsync();

                // Update balances
                foreach (var resource in document.ReceiptResources)
                {
                    await UpdateBalanceAsync(resource.ResourceId, resource.UnitOfMeasurementId, resource.Quantity);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                
                return document;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ReceiptDocument> UpdateReceiptDocumentAsync(ReceiptDocument document)
        {
            await ValidateReceiptDocumentAsync(document);
            
            var existingDocument = await _context.ReceiptDocuments
                .Include(d => d.ReceiptResources)
                .FirstOrDefaultAsync(d => d.Id == document.Id);
                
            if (existingDocument == null)
            {
                throw new ArgumentException("Документ не найден");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Revert old balances
                foreach (var oldResource in existingDocument.ReceiptResources)
                {
                    await UpdateBalanceAsync(oldResource.ResourceId, oldResource.UnitOfMeasurementId, -oldResource.Quantity);
                }

                // Update document
                document.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.ReceiptDocuments.Update(document);
                
                // Apply new balances
                foreach (var newResource in document.ReceiptResources)
                {
                    await UpdateBalanceAsync(newResource.ResourceId, newResource.UnitOfMeasurementId, newResource.Quantity);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                
                return document;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteReceiptDocumentAsync(int id)
        {
            var document = await _context.ReceiptDocuments
                .Include(d => d.ReceiptResources)
                .FirstOrDefaultAsync(d => d.Id == id);
                
            if (document == null)
            {
                throw new ArgumentException("Документ не найден");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Revert balances
                foreach (var resource in document.ReceiptResources)
                {
                    await UpdateBalanceAsync(resource.ResourceId, resource.UnitOfMeasurementId, -resource.Quantity);
                }

                _unitOfWork.ReceiptDocuments.Remove(document);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IEnumerable<ReceiptDocument>> GetReceiptDocumentsAsync(DocumentFilterDto? filter = null)
        {
            var query = _context.ReceiptDocuments
                .Include(d => d.ReceiptResources)
                    .ThenInclude(r => r.Resource)
                .Include(d => d.ReceiptResources)
                    .ThenInclude(r => r.UnitOfMeasurement)
                .AsQueryable();

            if (filter != null)
            {
                if (filter.DateFrom.HasValue)
                {
                    query = query.Where(d => d.Date >= filter.DateFrom.Value);
                }
                
                if (filter.DateTo.HasValue)
                {
                    query = query.Where(d => d.Date <= filter.DateTo.Value);
                }
                
                if (filter.DocumentNumbers.Any())
                {
                    query = query.Where(d => filter.DocumentNumbers.Contains(d.Number));
                }
                
                if (filter.ResourceIds.Any())
                {
                    query = query.Where(d => d.ReceiptResources.Any(r => filter.ResourceIds.Contains(r.ResourceId)));
                }
                
                if (filter.UnitOfMeasurementIds.Any())
                {
                    query = query.Where(d => d.ReceiptResources.Any(r => filter.UnitOfMeasurementIds.Contains(r.UnitOfMeasurementId)));
                }
            }

            return await query.OrderByDescending(d => d.Date).ToListAsync();
        }

        public async Task<ReceiptDocument?> GetReceiptDocumentByIdAsync(int id)
        {
            return await _context.ReceiptDocuments
                .Include(d => d.ReceiptResources)
                    .ThenInclude(r => r.Resource)
                .Include(d => d.ReceiptResources)
                    .ThenInclude(r => r.UnitOfMeasurement)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<ShipmentDocument> CreateShipmentDocumentAsync(ShipmentDocument document)
        {
            await ValidateShipmentDocumentAsync(document);
            
            document.CreatedAt = DateTime.UtcNow;
            document.UpdatedAt = DateTime.UtcNow;
            document.State = DocumentState.Draft;
            
            await _unitOfWork.ShipmentDocuments.AddAsync(document);
            await _unitOfWork.SaveChangesAsync();
            
            return document;
        }

        public async Task<ShipmentDocument> UpdateShipmentDocumentAsync(ShipmentDocument document)
        {
            await ValidateShipmentDocumentAsync(document);
            
            document.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.ShipmentDocuments.Update(document);
            await _unitOfWork.SaveChangesAsync();
            
            return document;
        }

        public async Task DeleteShipmentDocumentAsync(int id)
        {
            var document = await _context.ShipmentDocuments
                .Include(d => d.ShipmentResources)
                .FirstOrDefaultAsync(d => d.Id == id);
                
            if (document == null)
            {
                throw new ArgumentException("Документ не найден");
            }

            if (document.State == DocumentState.Signed)
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    // Return resources to balance
                    foreach (var resource in document.ShipmentResources)
                    {
                        await UpdateBalanceAsync(resource.ResourceId, resource.UnitOfMeasurementId, resource.Quantity);
                    }

                    _unitOfWork.ShipmentDocuments.Remove(document);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
            else
            {
                _unitOfWork.ShipmentDocuments.Remove(document);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<ShipmentDocument> SignShipmentDocumentAsync(int id)
        {
            var document = await _context.ShipmentDocuments
                .Include(d => d.ShipmentResources)
                .FirstOrDefaultAsync(d => d.Id == id);
                
            if (document == null)
            {
                throw new ArgumentException("Документ не найден");
            }
            
            if (document.State == DocumentState.Signed)
            {
                throw new InvalidOperationException("Документ уже подписан");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Check and update balances
                foreach (var resource in document.ShipmentResources)
                {
                    await UpdateBalanceAsync(resource.ResourceId, resource.UnitOfMeasurementId, -resource.Quantity);
                }

                document.State = DocumentState.Signed;
                document.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.ShipmentDocuments.Update(document);
                
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                
                return document;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ShipmentDocument> RevokeShipmentDocumentAsync(int id)
        {
            var document = await _context.ShipmentDocuments
                .Include(d => d.ShipmentResources)
                .FirstOrDefaultAsync(d => d.Id == id);
                
            if (document == null)
            {
                throw new ArgumentException("Документ не найден");
            }
            
            if (document.State != DocumentState.Signed)
            {
                throw new InvalidOperationException("Можно отозвать только подписанный документ");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Return resources to balance
                foreach (var resource in document.ShipmentResources)
                {
                    await UpdateBalanceAsync(resource.ResourceId, resource.UnitOfMeasurementId, resource.Quantity);
                }

                document.State = DocumentState.Revoked;
                document.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.ShipmentDocuments.Update(document);
                
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                
                return document;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IEnumerable<ShipmentDocument>> GetShipmentDocumentsAsync(DocumentFilterDto? filter = null)
        {
            var query = _context.ShipmentDocuments
                .Include(d => d.Client)
                .Include(d => d.ShipmentResources)
                    .ThenInclude(r => r.Resource)
                .Include(d => d.ShipmentResources)
                    .ThenInclude(r => r.UnitOfMeasurement)
                .AsQueryable();

            if (filter != null)
            {
                if (filter.DateFrom.HasValue)
                {
                    query = query.Where(d => d.Date >= filter.DateFrom.Value);
                }
                
                if (filter.DateTo.HasValue)
                {
                    query = query.Where(d => d.Date <= filter.DateTo.Value);
                }
                
                if (filter.DocumentNumbers.Any())
                {
                    query = query.Where(d => filter.DocumentNumbers.Contains(d.Number));
                }
                
                if (filter.ResourceIds.Any())
                {
                    query = query.Where(d => d.ShipmentResources.Any(r => filter.ResourceIds.Contains(r.ResourceId)));
                }
                
                if (filter.UnitOfMeasurementIds.Any())
                {
                    query = query.Where(d => d.ShipmentResources.Any(r => filter.UnitOfMeasurementIds.Contains(r.UnitOfMeasurementId)));
                }
            }

            return await query.OrderByDescending(d => d.Date).ToListAsync();
        }

        public async Task<ShipmentDocument?> GetShipmentDocumentByIdAsync(int id)
        {
            return await _context.ShipmentDocuments
                .Include(d => d.Client)
                .Include(d => d.ShipmentResources)
                    .ThenInclude(r => r.Resource)
                .Include(d => d.ShipmentResources)
                    .ThenInclude(r => r.UnitOfMeasurement)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task ValidateReceiptDocumentAsync(ReceiptDocument document)
        {
            // Check for duplicate document number
            var existingDocument = await _unitOfWork.ReceiptDocuments
                .ExistsAsync(d => d.Number == document.Number && d.Id != document.Id);
                
            if (existingDocument)
            {
                throw new InvalidOperationException($"Документ с номером '{document.Number}' уже существует");
            }
        }

        public async Task ValidateShipmentDocumentAsync(ShipmentDocument document)
        {
            // Check for duplicate document number
            var existingDocument = await _unitOfWork.ShipmentDocuments
                .ExistsAsync(d => d.Number == document.Number && d.Id != document.Id);
                
            if (existingDocument)
            {
                throw new InvalidOperationException($"Документ с номером '{document.Number}' уже существует");
            }
            
            // Check that document is not empty
            if (!document.ShipmentResources.Any())
            {
                throw new InvalidOperationException("Документ отгрузки не может быть пустым");
            }
        }

        public async Task<bool> CanDeleteResourceAsync(int resourceId)
        {
            var isUsedInBalance = await _unitOfWork.Balances.ExistsAsync(b => b.ResourceId == resourceId);
            var isUsedInReceipt = await _unitOfWork.ReceiptResources.ExistsAsync(r => r.ResourceId == resourceId);
            var isUsedInShipment = await _unitOfWork.ShipmentResources.ExistsAsync(r => r.ResourceId == resourceId);
            
            return !isUsedInBalance && !isUsedInReceipt && !isUsedInShipment;
        }

        public async Task<bool> CanDeleteUnitOfMeasurementAsync(int unitOfMeasurementId)
        {
            var isUsedInBalance = await _unitOfWork.Balances.ExistsAsync(b => b.UnitOfMeasurementId == unitOfMeasurementId);
            var isUsedInReceipt = await _unitOfWork.ReceiptResources.ExistsAsync(r => r.UnitOfMeasurementId == unitOfMeasurementId);
            var isUsedInShipment = await _unitOfWork.ShipmentResources.ExistsAsync(r => r.UnitOfMeasurementId == unitOfMeasurementId);
            
            return !isUsedInBalance && !isUsedInReceipt && !isUsedInShipment;
        }

        public async Task<bool> CanDeleteClientAsync(int clientId)
        {
            var isUsedInShipment = await _unitOfWork.ShipmentDocuments.ExistsAsync(d => d.ClientId == clientId);
            
            return !isUsedInShipment;
        }
    }
}
