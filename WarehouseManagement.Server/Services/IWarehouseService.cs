using WarehouseManagement.Shared.Models;
using WarehouseManagement.Shared.DTOs;

namespace WarehouseManagement.Server.Services
{
    public interface IWarehouseService
    {
        // Balance operations
        Task<IEnumerable<Balance>> GetBalanceAsync(BalanceFilterDto? filter = null);
        Task UpdateBalanceAsync(int resourceId, int unitOfMeasurementId, decimal quantityChange);
        
        // Receipt document operations
        Task<ReceiptDocument> CreateReceiptDocumentAsync(ReceiptDocument document);
        Task<ReceiptDocument> UpdateReceiptDocumentAsync(ReceiptDocument document);
        Task DeleteReceiptDocumentAsync(int id);
        Task<IEnumerable<ReceiptDocument>> GetReceiptDocumentsAsync(DocumentFilterDto? filter = null);
        Task<ReceiptDocument?> GetReceiptDocumentByIdAsync(int id);
        
        // Shipment document operations
        Task<ShipmentDocument> CreateShipmentDocumentAsync(ShipmentDocument document);
        Task<ShipmentDocument> UpdateShipmentDocumentAsync(ShipmentDocument document);
        Task DeleteShipmentDocumentAsync(int id);
        Task<ShipmentDocument> SignShipmentDocumentAsync(int id);
        Task<ShipmentDocument> RevokeShipmentDocumentAsync(int id);
        Task<IEnumerable<ShipmentDocument>> GetShipmentDocumentsAsync(DocumentFilterDto? filter = null);
        Task<ShipmentDocument?> GetShipmentDocumentByIdAsync(int id);
        
        // Validation
        Task ValidateReceiptDocumentAsync(ReceiptDocument document);
        Task ValidateShipmentDocumentAsync(ShipmentDocument document);
        Task<bool> CanDeleteResourceAsync(int resourceId);
        Task<bool> CanDeleteUnitOfMeasurementAsync(int unitOfMeasurementId);
        Task<bool> CanDeleteClientAsync(int clientId);
    }
}
