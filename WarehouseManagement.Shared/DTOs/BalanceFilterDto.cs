namespace WarehouseManagement.Shared.DTOs
{
    /// <summary>
    /// Фильтр для баланса склада
    /// </summary>
    public class BalanceFilterDto
    {
        /// <summary>
        /// Идентификаторы ресурсов
        /// </summary>
        public List<int> ResourceIds { get; set; } = new();
        
        /// <summary>
        /// Идентификаторы единиц измерения
        /// </summary>
        public List<int> UnitOfMeasurementIds { get; set; } = new();
    }
}
