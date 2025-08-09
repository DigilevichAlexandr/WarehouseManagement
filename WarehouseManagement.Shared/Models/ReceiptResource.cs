using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Shared.Models
{
    /// <summary>
    /// Ресурс поступления
    /// </summary>
    public class ReceiptResource
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Идентификатор документа поступления
        /// </summary>
        public int ReceiptDocumentId { get; set; }
        
        /// <summary>
        /// Документ поступления
        /// </summary>
        public ReceiptDocument ReceiptDocument { get; set; } = null!;
        
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
