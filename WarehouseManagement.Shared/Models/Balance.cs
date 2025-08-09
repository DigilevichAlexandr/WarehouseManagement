using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Shared.Models
{
    /// <summary>
    /// Баланс ресурсов на складе
    /// </summary>
    public class Balance
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Идентификатор ресурса
        /// </summary>
        public int ResourceId { get; set; }
        
        /// <summary>
        /// Ресурс
        /// </summary>
        public Resource? Resource { get; set; }
        
        /// <summary>
        /// Идентификатор единицы измерения
        /// </summary>
        public int UnitOfMeasurementId { get; set; }
        
        /// <summary>
        /// Единица измерения
        /// </summary>
        public UnitOfMeasurement? UnitOfMeasurement { get; set; }
        
        /// <summary>
        /// Количество
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; }
        
        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
