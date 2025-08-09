using Microsoft.EntityFrameworkCore.Storage;
using WarehouseManagement.Server.Data;
using WarehouseManagement.Shared.Models;

namespace WarehouseManagement.Server.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WarehouseContext _context;
        private IDbContextTransaction? _transaction;
        
        private IGenericRepository<Resource>? _resources;
        private IGenericRepository<UnitOfMeasurement>? _unitsOfMeasurement;
        private IGenericRepository<Client>? _clients;
        private IGenericRepository<Balance>? _balances;
        private IGenericRepository<ReceiptDocument>? _receiptDocuments;
        private IGenericRepository<ReceiptResource>? _receiptResources;
        private IGenericRepository<ShipmentDocument>? _shipmentDocuments;
        private IGenericRepository<ShipmentResource>? _shipmentResources;

        public UnitOfWork(WarehouseContext context)
        {
            _context = context;
        }

        public IGenericRepository<Resource> Resources =>
            _resources ??= new GenericRepository<Resource>(_context);

        public IGenericRepository<UnitOfMeasurement> UnitsOfMeasurement =>
            _unitsOfMeasurement ??= new GenericRepository<UnitOfMeasurement>(_context);

        public IGenericRepository<Client> Clients =>
            _clients ??= new GenericRepository<Client>(_context);

        public IGenericRepository<Balance> Balances =>
            _balances ??= new GenericRepository<Balance>(_context);

        public IGenericRepository<ReceiptDocument> ReceiptDocuments =>
            _receiptDocuments ??= new GenericRepository<ReceiptDocument>(_context);

        public IGenericRepository<ReceiptResource> ReceiptResources =>
            _receiptResources ??= new GenericRepository<ReceiptResource>(_context);

        public IGenericRepository<ShipmentDocument> ShipmentDocuments =>
            _shipmentDocuments ??= new GenericRepository<ShipmentDocument>(_context);

        public IGenericRepository<ShipmentResource> ShipmentResources =>
            _shipmentResources ??= new GenericRepository<ShipmentResource>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
