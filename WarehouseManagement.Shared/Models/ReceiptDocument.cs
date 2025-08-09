using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Shared.Models
{
    /// <summary>
    /// Документ поступления
    /// </summary>
    public class ReceiptDocument
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
        /// Дата документа
        /// </summary>
        public DateTime Date { get; set; } = DateTime.Today;
        
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Дата обновления
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Ресурсы поступления
        /// </summary>
        public List<ReceiptResource> ReceiptResources { get; set; } = new();
    }
}
