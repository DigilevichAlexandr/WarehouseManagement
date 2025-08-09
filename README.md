# Система управления складом

Приложение для автоматизации работы склада, разработанное на ASP.NET Core и Blazor WebAssembly.

## Архитектура

- **Серверная часть**: ASP.NET Core Web API с Entity Framework Core
- **Клиентская часть**: Blazor WebAssembly
- **База данных**: SQL Server Express
- **Общие модели**: Shared библиотека

## Функциональность

### Основные сущности
- **Ресурсы** - материалы на складе
- **Единицы измерения** - кг, м, шт и т.д.
- **Клиенты** - получатели отгрузок
- **Баланс** - текущие остатки на складе
- **Документы поступления** - приход товаров
- **Документы отгрузки** - расход товаров

### Бизнес-правила
- Уникальность наименований ресурсов, единиц измерения, клиентов и номеров документов
- Архивирование вместо удаления для используемых сущностей
- Автоматическое обновление баланса при операциях с документами
- Контроль достаточности ресурсов при отгрузке
- Подписание и отзыв документов отгрузки

### Фильтрация
- **Склад**: по ресурсам и единицам измерения
- **Документы**: по датам, номерам, ресурсам и единицам измерения
- Серверная фильтрация с поддержкой множественного выбора

## Запуск приложения

### Предварительные требования
- .NET 8.0 SDK
- SQL Server Express (можно скачать с сайта Microsoft)

### Настройка SQL Server Express
1. Скачайте и установите SQL Server Express с сайта Microsoft
2. Убедитесь, что экземпляр SQLEXPRESS запущен
3. Для проверки подключения выполните в командной строке:
   ```cmd
   sqlcmd -S .\SQLEXPRESS -E
   ```
4. Если возникают проблемы с подключением, проверьте службы Windows (services.msc):
   - SQL Server (SQLEXPRESS) должна быть запущена
   - SQL Server Browser (опционально для локального подключения)

### Альтернативные строки подключения
Если стандартная строка подключения не работает, попробуйте изменить в `appsettings.json`:

**Для SQL Server Express с именованным экземпляром:**
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=WarehouseManagement;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
```

**Для полной версии SQL Server:**
```json
"DefaultConnection": "Server=localhost;Database=WarehouseManagement;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
```

**Для SQL Server с логином и паролем:**
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=WarehouseManagement;User Id=sa;Password=yourpassword;MultipleActiveResultSets=true;TrustServerCertificate=true"
```

### 1. Запуск серверной части
```bash
cd WarehouseManagement.Server
dotnet run
```
Сервер запустится на https://localhost:7001

### 2. Запуск клиентской части
```bash
cd WarehouseManagement.Client
dotnet run
```
Клиент запустится на https://localhost:5001

### 3. Альтернативный запуск
Для одновременного запуска обеих частей используйте два терминала или Visual Studio.

## База данных

При первом запуске база данных будет создана автоматически с тестовыми данными:
- 5 ресурсов (стальной лист, алюминиевый профиль и др.)
- 5 единиц измерения (кг, м, шт, м², л)
- 4 клиента
- 2 документа поступления с балансом
- 1 документ отгрузки в статусе "Черновик"

## Структура проекта

```
WarehouseManagement/
├── WarehouseManagement.Server/     # ASP.NET Core Web API
│   ├── Controllers/                # API контроллеры
│   ├── Data/                      # DbContext и инициализация
│   ├── Repositories/              # Репозитории и Unit of Work
│   └── Services/                  # Бизнес-логика
├── WarehouseManagement.Client/     # Blazor WebAssembly
│   ├── Pages/                     # Страницы приложения
│   ├── Layout/                    # Разметка и навигация
│   └── Services/                  # HTTP клиент для API
└── WarehouseManagement.Shared/     # Общие модели и DTOs
    ├── Models/                    # Сущности базы данных
    ├── Enums/                     # Перечисления
    └── DTOs/                      # Объекты передачи данных
```

## API Endpoints

### Ресурсы
- `GET /api/resources` - Все ресурсы
- `GET /api/resources/active` - Активные ресурсы
- `POST /api/resources` - Создать ресурс
- `PUT /api/resources/{id}` - Обновить ресурс
- `DELETE /api/resources/{id}` - Удалить ресурс
- `POST /api/resources/{id}/archive` - Архивировать
- `POST /api/resources/{id}/restore` - Восстановить

### Единицы измерения
- `GET /api/unitsofmeasurement` - Все единицы
- `GET /api/unitsofmeasurement/active` - Активные единицы
- Аналогичные операции CRUD

### Клиенты
- `GET /api/clients` - Все клиенты
- `GET /api/clients/active` - Активные клиенты
- Аналогичные операции CRUD

### Баланс
- `GET /api/balance` - Остатки на складе с фильтрацией

### Документы поступления
- `GET /api/receiptdocuments` - Все документы с фильтрацией
- `POST /api/receiptdocuments` - Создать документ
- `PUT /api/receiptdocuments/{id}` - Обновить документ
- `DELETE /api/receiptdocuments/{id}` - Удалить документ

### Документы отгрузки
- `GET /api/shipmentdocuments` - Все документы с фильтрацией
- `POST /api/shipmentdocuments` - Создать документ
- `PUT /api/shipmentdocuments/{id}` - Обновить документ
- `DELETE /api/shipmentdocuments/{id}` - Удалить документ
- `POST /api/shipmentdocuments/{id}/sign` - Подписать документ
- `POST /api/shipmentdocuments/{id}/revoke` - Отозвать документ

## Используемые технологии

- **ASP.NET Core 8.0** - серверный фреймворк
- **Blazor WebAssembly** - клиентский SPA фреймворк
- **Entity Framework Core** - ORM для работы с базой данных
- **SQL Server** - система управления базами данных
- **Bootstrap 5** - CSS фреймворк для UI
- **Repository Pattern** - паттерн доступа к данным
- **Unit of Work** - паттерн для управления транзакциями

## Особенности реализации

1. **Транзакционность** - все операции с балансом выполняются в транзакциях
2. **Валидация** - проверка бизнес-правил на серверной стороне
3. **Обработка ошибок** - информативные сообщения об ошибках
4. **Фильтрация** - серверная фильтрация с поддержкой сложных запросов
5. **Архивирование** - безопасное скрытие данных вместо удаления
6. **Автоматическая миграция** - создание БД при первом запуске
