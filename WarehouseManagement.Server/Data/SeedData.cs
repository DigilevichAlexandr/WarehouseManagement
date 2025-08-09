using WarehouseManagement.Shared.Models;
using WarehouseManagement.Shared.Enums;

namespace WarehouseManagement.Server.Data
{
    public static class SeedData
    {
        public static void Initialize(WarehouseContext context)
        {
            if (context.Resources.Any())
                return; // DB has been seeded

            // Seed Resources
            var resources = new[]
            {
                new Resource { Name = "Стальной лист", State = EntityState.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Resource { Name = "Алюминиевый профиль", State = EntityState.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Resource { Name = "Пластиковые трубы", State = EntityState.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Resource { Name = "Медная проволока", State = EntityState.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Resource { Name = "Резиновые уплотнители", State = EntityState.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };
            context.Resources.AddRange(resources);

            // Seed Units of Measurement
            var units = new[]
            {
                new UnitOfMeasurement { Name = "кг", State = EntityState.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new UnitOfMeasurement { Name = "м", State = EntityState.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new UnitOfMeasurement { Name = "шт", State = EntityState.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new UnitOfMeasurement { Name = "м²", State = EntityState.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new UnitOfMeasurement { Name = "л", State = EntityState.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };
            context.UnitsOfMeasurement.AddRange(units);

            // Seed Clients
            var clients = new[]
            {
                new Client { Name = "ООО \"Строймонтаж\"", Address = "г. Москва, ул. Ленина, 15", State = EntityState.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Client { Name = "АО \"ПромСтрой\"", Address = "г. Санкт-Петербург, пр. Невский, 100", State = EntityState.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Client { Name = "ИП Иванов А.С.", Address = "г. Новосибирск, ул. Красная, 25", State = EntityState.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Client { Name = "ООО \"МетТорг\"", Address = "г. Екатеринбург, ул. Металлистов, 8", State = EntityState.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };
            context.Clients.AddRange(clients);

            context.SaveChanges();

            // Seed some initial balance (example receipt documents)
            var receiptDoc1 = new ReceiptDocument
            {
                Number = "П-001",
                Date = DateTime.Today.AddDays(-10),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ReceiptResources = new List<ReceiptResource>
                {
                    new ReceiptResource { ResourceId = 1, UnitOfMeasurementId = 1, Quantity = 1000 }, // Стальной лист, 1000 кг
                    new ReceiptResource { ResourceId = 2, UnitOfMeasurementId = 2, Quantity = 500 }   // Алюминиевый профиль, 500 м
                }
            };

            var receiptDoc2 = new ReceiptDocument
            {
                Number = "П-002",
                Date = DateTime.Today.AddDays(-5),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ReceiptResources = new List<ReceiptResource>
                {
                    new ReceiptResource { ResourceId = 3, UnitOfMeasurementId = 3, Quantity = 200 }, // Пластиковые трубы, 200 шт
                    new ReceiptResource { ResourceId = 4, UnitOfMeasurementId = 1, Quantity = 50 },  // Медная проволока, 50 кг
                    new ReceiptResource { ResourceId = 5, UnitOfMeasurementId = 3, Quantity = 1000 } // Резиновые уплотнители, 1000 шт
                }
            };

            context.ReceiptDocuments.AddRange(receiptDoc1, receiptDoc2);
            context.SaveChanges();

            // Create balances based on receipts
            var balances = new[]
            {
                new Balance { ResourceId = 1, UnitOfMeasurementId = 1, Quantity = 1000, UpdatedAt = DateTime.UtcNow },
                new Balance { ResourceId = 2, UnitOfMeasurementId = 2, Quantity = 500, UpdatedAt = DateTime.UtcNow },
                new Balance { ResourceId = 3, UnitOfMeasurementId = 3, Quantity = 200, UpdatedAt = DateTime.UtcNow },
                new Balance { ResourceId = 4, UnitOfMeasurementId = 1, Quantity = 50, UpdatedAt = DateTime.UtcNow },
                new Balance { ResourceId = 5, UnitOfMeasurementId = 3, Quantity = 1000, UpdatedAt = DateTime.UtcNow }
            };
            context.Balances.AddRange(balances);

            // Add a sample shipment document
            var shipmentDoc = new ShipmentDocument
            {
                Number = "О-001",
                ClientId = 1,
                Date = DateTime.Today.AddDays(-2),
                State = DocumentState.Draft,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ShipmentResources = new List<ShipmentResource>
                {
                    new ShipmentResource { ResourceId = 1, UnitOfMeasurementId = 1, Quantity = 100 }, // Стальной лист, 100 кг
                    new ShipmentResource { ResourceId = 3, UnitOfMeasurementId = 3, Quantity = 50 }   // Пластиковые трубы, 50 шт
                }
            };

            context.ShipmentDocuments.Add(shipmentDoc);
            context.SaveChanges();
        }
    }
}
