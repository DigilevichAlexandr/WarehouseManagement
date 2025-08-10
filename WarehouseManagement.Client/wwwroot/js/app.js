// Инициализация tooltip'ов Bootstrap
function initializeTooltips() {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Инициализация всех интерактивных элементов
function initializeInteractiveElements() {
    initializeTooltips();
    
    // Добавляем анимации для кнопок
    document.querySelectorAll('.btn').forEach(button => {
        button.addEventListener('click', function() {
            // Добавляем эффект нажатия
            this.style.transform = 'scale(0.95)';
            setTimeout(() => {
                this.style.transform = '';
            }, 150);
        });
    });
}

// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', function() {
    initializeInteractiveElements();
});

// Экспортируем функции для использования в Blazor
window.warehouseApp = {
    initializeTooltips: initializeTooltips,
    initializeInteractiveElements: initializeInteractiveElements
};
