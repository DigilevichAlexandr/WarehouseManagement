using WarehouseManagement.Shared.Models;

namespace WarehouseManagement.Server.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Resource> Resources { get; }
        IGenericRepository<UnitOfMeasurement> UnitsOfMeasurement { get; }
        IGenericRepository<Client> Clients { get; }
        IGenericRepository<Balance> Balances { get; }
        IGenericRepository<ReceiptDocument> ReceiptDocuments { get; }
        IGenericRepository<ReceiptResource> ReceiptResources { get; }
        IGenericRepository<ShipmentDocument> ShipmentDocuments { get; }
        IGenericRepository<ShipmentResource> ShipmentResources { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
