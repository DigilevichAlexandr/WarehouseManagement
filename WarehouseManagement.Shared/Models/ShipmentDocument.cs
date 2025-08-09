using WarehouseManagement.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Shared.Models
{
    /// <summary>
    /// Документ отгрузки
    /// </summary>
    public class ShipmentDocument
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Номер документа
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Number { get; set; } = string.Empty;
        
        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        public int ClientId { get; set; }
        
        /// <summary>
        /// Клиент
        /// </summary>
        public Client? Client { get; set; }
        
        /// <summary>
        /// Дата документа
        /// </summary>
        public DateTime Date { get; set; } = DateTime.Today;
        
        /// <summary>
        /// Состояние документа
        /// </summary>
        public DocumentState State { get; set; } = DocumentState.Draft;
        
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Дата обновления
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Ресурсы отгрузки
        /// </summary>
        public List<ShipmentResource> ShipmentResources { get; set; } = new();
    }
}
