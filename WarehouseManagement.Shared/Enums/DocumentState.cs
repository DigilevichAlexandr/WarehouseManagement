namespace WarehouseManagement.Shared.Enums
{
    /// <summary>
    /// Состояние документа отгрузки
    /// </summary>
    public enum DocumentState
    {
        /// <summary>
        /// Черновик
        /// </summary>
        Draft = 1,
        
        /// <summary>
        /// Подписан
        /// </summary>
        Signed = 2,
        
        /// <summary>
        /// Отозван
        /// </summary>
        Revoked = 3
    }
}
