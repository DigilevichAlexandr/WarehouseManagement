using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Shared.Models
{
    /// <summary>
    /// Ресурс отгрузки
    /// </summary>
    public class ShipmentResource
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Идентификатор документа отгрузки
        /// </summary>
        public int ShipmentDocumentId { get; set; }
        
        /// <summary>
        /// Документ отгрузки
        /// </summary>
        public ShipmentDocument ShipmentDocument { get; set; } = null!;
        
        /// <summary>
        /// Идентификатор ресурса
        /// </summary>
        public int ResourceId { get; set; }
        
        /// <summary>
        /// Ресурс
        /// </summary>
        public Resource Resource { get; set; } = null!;
        
        /// <summary>
        /// Идентификатор единицы измерения
        /// </summary>
        public int UnitOfMeasurementId { get; set; }
        
        /// <summary>
        /// Единица измерения
        /// </summary>
        public UnitOfMeasurement UnitOfMeasurement { get; set; } = null!;
        
        /// <summary>
        /// Количество
        /// </summary>
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
    }
}
