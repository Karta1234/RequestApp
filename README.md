# RequestApp

Сервис для заявок сотрудников с обработкой бухгалтером. ASP.NET Core 10 + PostgreSQL + EF Core 10.

## Стек

- .NET 10 (ASP.NET Core Web API)
- PostgreSQL + `Npgsql.EntityFrameworkCore.PostgreSQL` 10.x
- EF Core 10 (Code First + `HasData` сидинг)
- `Microsoft.AspNetCore.OpenApi` (OpenAPI 3.0 document)
- `EFCore.NamingConventions` — snake_case в БД

## Структура

```
Auth/                — HeaderAuthHandler, ICurrentUser
Controllers/         — RequestController, HealthController
Data/                — AppDbContext, Configurations/
Domain/              — Request, RequestType, RequestStatus, RequestStatusHistory
                       Enums/RoleEnums.cs, RequestStatuses (статические инстансы)
Dtos/                — CreateRequestDto, RequestDto, RequestMappings (Expression)
Infrastructure/      — GlobalExceptionHandler, Exceptions
Migrations/          — EF Core миграции
Services/            — IRequestService, RequestService
Program.cs           — composition root
```

## Быстрый старт

### Требования

- .NET 10 SDK
- PostgreSQL 14+ запущен локально на `localhost:5432`
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

### Запуск

```bash
# Применить миграции (создаст БД и таблицы, засеет статусы)
dotnet ef database update

# Сидинг типов заявок — вручную, миграцией не покрыто
psql -U postgres -d requestapp -c "INSERT INTO request_types (name) VALUES ('Laptop'), ('Vacation'), ('Custom');"

# Запустить
dotnet run
```

Connection string в `appsettings.json` → `ConnectionStrings.Default`.

После старта:

- API: `http://localhost:5211`
- OpenAPI JSON: `http://localhost:5211/openapi/v1.json`

### Сидинг

| Сущность        | Метод                                    | Где                                |
| --------------- | ---------------------------------------- | ---------------------------------- |
| `RequestStatus` | `HasData` в `RequestStatusConfiguration` | Засевается миграцией автоматически |
| `RequestType`   | Вручную через SQL                        | Нет сидинга в коде                 |

## Аутентификация

Кастомная схема через HTTP-заголовки (`Auth/HeaderAuthHandler.cs`):

| Заголовок   | Тип                                           | Пример       |
| ----------- | --------------------------------------------- | ------------ |
| `X-User-Id` | int                                           | `42`         |
| `X-Role`    | `Employee` \| `Accountant` (case-insensitive) | `Accountant` |

| Ситуация                             | Ответ            |
| ------------------------------------ | ---------------- |
| Заголовков нет / невалидны           | 401 Unauthorized |
| Заголовки валидны, роли недостаточно | 403 Forbidden    |

В production предполагается замена на JWT — меняется только handler, всё остальное остаётся.

## Глобальная обработка ошибок

`Infrastructure/GlobalExceptionHandler.cs` (через `IExceptionHandler`) маппит:

| Exception           | HTTP         |
| ------------------- | ------------ |
| `NotFoundException` | 404          |
| `ConflictException` | 409          |
| остальные           | дефолт (500) |

Тело ответа: `{ "status": <int>, "error": "<message>" }`.

## Endpoints

См. подробное описание через OpenAPI document. Краткая сводка:

### Health (Anonymous)

| Метод | URL           | Описание               |
| ----- | ------------- | ---------------------- |
| GET   | `/api/health` | Liveness + проверка БД |

### Requests

| Метод | URL                                          | Auth                           | Описание                                              |
| ----- | -------------------------------------------- | ------------------------------ | ----------------------------------------------------- |
| POST  | `/api/request`                               | Any                            | Создать заявку (`EmployeeId` берётся из current user) |
| GET   | `/api/request`                               | Accountant                     | Список всех заявок                                    |
| GET   | `/api/request/my`                            | Any                            | Заявки текущего пользователя                          |
| GET   | `/api/request/{id}`                          | Any (свои); Accountant (любые) | Заявка по id; чужие → 404                             |
| PATCH | `/api/request/{id}/status?newStatusId=<int>` | Accountant                     | Сменить статус + записать в историю                   |

## Бизнес-правила

- При создании заявки автоматически выставляется статус `New` (через конструктор `Request`).
- Уникальный частичный индекс: `(employee_id, type_id)` где `is_active = true` — нельзя создать вторую активную заявку того же типа.
- Terminal-статусы (`Approved`, `Rejected`) выставляют `is_active = false`.
- Смена статуса завершённой заявки запрещена (409 Conflict).
- При смене статуса создаётся запись в `request_status_histories` в одной транзакции с обновлением заявки.

## Разработка

```bash
# Новая миграция
dotnet ef migrations add <Name> --output-dir Migrations

# Откатить последнюю миграцию (если не применена)
dotnet ef migrations remove

# Сгенерировать SQL без применения
dotnet ef migrations script

# Проверить статус миграций
dotnet ef migrations list
```
