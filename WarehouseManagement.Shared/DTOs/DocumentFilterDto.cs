namespace WarehouseManagement.Shared.DTOs
{
    /// <summary>
    /// Фильтр для документов
    /// </summary>
    public class DocumentFilterDto
    {
        /// <summary>
        /// Дата начала периода
        /// </summary>
        public DateTime? DateFrom { get; set; }
        
        /// <summary>
        /// Дата окончания периода
        /// </summary>
        public DateTime? DateTo { get; set; }
        
        /// <summary>
        /// Номера документов
        /// </summary>
        public List<string> DocumentNumbers { get; set; } = new();
        
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
