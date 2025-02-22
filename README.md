# API example

Проект представляет собой `RESTful` API для управления пользователями и аутентификацией, построенный на ASP.NET Core 8. Реализована JWT-аутентификация, управление пользователями через ASP.NET Core Identity и слоистая архитектура.

## 🧅 Архитектура

### Слои приложения:
1. **Domain**  
   - Сущности предметной области
   - Интерфейсы репозиториев
   - Бизнес-правила и исключения

2. **Application**  
   - CQRS (MediatR)
   - Валидация (FluentValidation)

3. **Infrastructure**  
   - Реализации репозиториев
   - Entity Framework Core (PostgreSQL)
   - ASP.NET Core Identity
   - JWT-аутентификация

4. **API**  
   - Контроллеры
   - DI-контейнер

## 🔐 Аутентификация
### Приложение использует JWT для аутентификации пользователей. Процесс аутентификации включает следующие шаги:
   - Регистрация пользователя: Пользователь регистрируется, указывая email и пароль. Пароль хэшируется и сохраняется в базе данных.
   - Вход в систему: Пользователь вводит email и пароль. Пароль проверяется путем сравнения его хэша с сохраненным хэшем.
   - Генерация JWT: При успешной проверке пароля генерируется JWT, который отправляется клиенту.
   - Использование JWT: Клиент включает JWT в заголовок Authorization при обращении к защищенным ресурсам
 
### Реализация:
- **JWT-токены** для защиты API-эндпоинтов
- **ASP.NET Core Identity** для:
  - Хранения учетных данных
  - Управления ролями
  - Хеширования паролей

```csharp
// Пример конфигурации JWT
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
```

  
## 💉 Dependency Injection
### Ключевые сервисы:

```csharp
// Регистрация слоев
services.AddScoped<IUserRepository, UserRepository>();
services.AddTransient<ITokenService, JwtTokenService>();
services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// MediatR
services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(ApplicationLayer).Assembly));

// Identity
services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
```

## Контроллеры

### UserController

`UserController` обрабатывает HTTP-запросы, связанные с пользователями. Он использует MediatR для отправки команд и запросов, что обеспечивает четкое разделение ответственности и улучшает тестируемость.

#### Методы

- **LoginAsync**: Обрабатывает POST-запросы на `api/users/login`. Принимает объект `LoginQuery` и возвращает результат аутентификации.

- **Register**: Обрабатывает POST-запросы на `api/users/registration`. Принимает объект `RegistrationCommand` и регистрирует нового пользователя.

- **CreateUserAsync**: Обрабатывает POST-запросы на `api/users`. Принимает объект `CreateUserCommand` и создает нового пользователя. Возвращает созданного пользователя или ошибку, если что-то пошло не так.
