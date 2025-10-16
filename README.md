# WarehouseApp
Веб-приложение для управления складом товаров. Использует ASP.NET Core, EF Core, SQLite и CsvHelper.

## Установка
1. Клонируйте репозиторий: `git clone <URL>`
2. Перейдите в реп WarehouseApp: cd Warehouse
3. Установите зависимости: `dotnet restore`
4. Примените миграции: `dotnet ef database update --context WarehouseContext`
5. Запустите: `dotnet run`

## Функционал
- Просмотр товаров с фильтрами/сортировкой.
- Добавление/редактирование через Razor Pages или API.
- Загрузка данных из CSV (`wwwroot/warehouse.csv`).
