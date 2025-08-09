using WarehouseManagement.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Shared.Models
{
    /// <summary>
    /// Клиент
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Наименование
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Адрес
        /// </summary>
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;
        
        /// <summary>
        /// Состояние
        /// </summary>
        public EntityState State { get; set; } = EntityState.Active;
        
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Дата обновления
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
