# Решение проблем

## Проблема: Процесс блокирует файлы при сборке

Если при запуске `dotnet run` вы видите ошибку:
```
The process cannot access the file because it is being used by another process. "WarehouseManagement.Server.exe" блокирует этот файл
```

### Решение:

1. **Остановить процесс через диспетчер задач:**
   - Нажмите `Ctrl + Shift + Esc`
   - Найдите процесс `WarehouseManagement.Server.exe`
   - Нажмите "Завершить задачу"

2. **Или через командную строку (с правами администратора):**
   ```cmd
   taskkill /F /IM WarehouseManagement.Server.exe
   ```

3. **Очистить временные файлы:**
   ```bash
   dotnet clean
   ```

4. **Запустить заново:**
   ```bash
   dotnet run
   ```

## Проблема: Нет данных на сайте

### Возможные причины и решения:

1. **Сервер не запущен:**
   - Убедитесь, что сервер запущен: `dotnet run` в папке `WarehouseManagement.Server`
   - Проверьте, что сервер доступен по адресу https://localhost:7001

2. **Проблемы с базой данных:**
   - Убедитесь, что SQL Server Express установлен и запущен
   - Проверьте строку подключения в `appsettings.json`
   - Проверьте службы Windows (services.msc): SQL Server (SQLEXPRESS)

3. **Проблемы с CORS:**
   - Сервер должен быть настроен на порт 7001
   - Клиент должен быть настроен на порт 5001
   - Проверьте настройки CORS в `Program.cs`

4. **Ошибки в консоли браузера:**
   - Откройте DevTools (F12) и проверьте консоль на ошибки
   - Проверьте вкладку Network на HTTP ошибки

## Правильная последовательность запуска:

1. **Запустить сервер (первый терминал):**
   ```bash
   cd WarehouseManagement.Server
   dotnet run
   ```
   Дождитесь сообщения: "Now listening on: https://localhost:7001"

2. **Запустить клиент (второй терминал):**
   ```bash
   cd WarehouseManagement.Client
   dotnet run
   ```
   Дождитесь сообщения: "Now listening on: https://localhost:5001"

3. **Открыть браузер:**
   - Перейдите на https://localhost:5001
   - Примите сертификат безопасности если потребуется

## Проверка API сервера:

Проверьте доступность API:
```bash
curl https://localhost:7001/api/resources
```

Или откройте в браузере: https://localhost:7001/swagger

## Проверка базы данных:

1. **Подключитесь к SQL Server:**
   ```cmd
   sqlcmd -S .\SQLEXPRESS -E
   ```

2. **Проверьте базу данных:**
   ```sql
   USE WarehouseManagement
   SELECT COUNT(*) FROM Resources
   GO
   ```

Должно быть 5 ресурсов.

## Альтернативные порты:

Если порты заняты, измените в:
- `WarehouseManagement.Server/Properties/launchSettings.json`
- `WarehouseManagement.Client/Program.cs` (BaseAddress)

## Логи и отладка:

Проверьте логи в консоли сервера для диагностики ошибок подключения к БД или других проблем.
