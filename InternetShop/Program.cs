using FluentResults;
using FluentValidation;
using InternetShop.Application.BusinessLogic;
using InternetShop.Application.BusinessLogic.Cart;
using InternetShop.Application.BusinessLogic.Product;
using InternetShop.Application.BusinessLogic.User;
using InternetShop.Application.BusinessLogic.User.DeleteUser;
using InternetShop.Application.BusinessLogic.User.GetAllUsers;
using InternetShop.Application.BusinessLogic.User.GetCurrentUser;
using InternetShop.Application.BusinessLogic.User.Login;
using InternetShop.Application.BusinessLogic.User.Registration;
using InternetShop.Application.BusinessLogic.User.UpdateUser;
using InternetShop.Database;
using InternetShop.Database.Models;
using InternetShop.Database.Models.Authentication;
using InternetShop.Database.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(loggingBuilder => {
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
    loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
});

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", builder => {
        builder.WithOrigins("https://localhost:7065")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});


/*// Регистрация Identity
builder.Services.AddIdentity<UserEntity, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders()
    .AddUserStore<UserRepository>();*/

// В методе ConfigureServices
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException("JWT Key must be configured and be at least 32 characters long");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("Management", policy =>
        policy.RequireRole("Admin", "Manager"));

    options.AddPolicy("StaffOnly", policy =>
        policy.RequireRole("Admin", "Manager", "Employee"));

    options.AddPolicy("ClientOnly", policy =>
        policy.RequireRole("Client"));
});

//Конфигурация MongoDB
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var mongoConfig = builder.Configuration.GetSection("MongoDb");
var mongoConnectionString = mongoConfig["ConnectionUrl"] ??
    throw new ArgumentNullException("MongoDb:ConnectionUrl is not configured");
var mongoDatabaseName = mongoConfig["DatabaseName"] ??
    throw new ArgumentNullException("MongoDb:DatabaseName is not configured");

var mongoClient = new MongoClient(mongoConnectionString);
var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);
builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);

// Конфигурация Redis
var redisConnectionString = builder.Configuration["Redis:ConnectionString"] ??
    throw new ArgumentNullException("Redis:ConnectionString is not configured");
var redisInstanceName = builder.Configuration["Redis:InstanceName"] ?? "DefaultInstance";

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = redisInstanceName;
});

// Регистрация контекста БД
var pgConnectionString = builder.
    Configuration.GetConnectionString("InternetShopDbContext") ??
    throw new ArgumentNullException("InternetShopDbContext connection string is not configured");
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseNpgsql(pgConnectionString);
});


// Регистрация сервисов
builder.Services.AddScoped<IdentityRepository>();
// 1. Сначала регистрируем все зависимости Identity
builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
builder.Services.AddScoped<IUserStore<UserEntity>, IdentityRepository>();
builder.Services.AddScoped<IUserPasswordStore<UserEntity>, IdentityRepository>();
builder.Services.AddScoped<IUserEmailStore<UserEntity>, IdentityRepository>();
//builder.Services.AddScoped<IUserRoleStore<UserEntity>, IdentityRepository>();

// 2. Настраиваем IdentityCore
builder.Services.AddIdentityCore<UserEntity>(options =>
{
    // Настройки Identity, если нужны
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddUserStore<IdentityRepository>()
.AddRoles<IdentityRole<Guid>>()
.AddRoleStore<RoleStore<IdentityRole<Guid>, UserDbContext, Guid>>()
.AddRoleManager<RoleManager<IdentityRole<Guid>>>()
.AddSignInManager<SignInManager<UserEntity>>();

// 3. Регистрируем UserManager и другие менеджеры
/*builder.Services.AddScoped<UserManager<UserEntity>>();
builder.Services.AddScoped<SignInManager<UserEntity>>();
builder.Services.AddScoped<RoleManager<IdentityRole<Guid>>>();*/

// 4. Регистрируем UserRepository только для его интерфейса
//builder.Services.AddScoped<IUserRepository, UserRepository>();

// 5. Регистрируем другие сервисы
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<ITokenService, JWTTokenService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

//
/*builder.Services.AddScoped<UserManager<UserEntity>>();
builder.Services.AddScoped<SignInManager<UserEntity>>();
builder.Services.AddScoped<RoleManager<IdentityRole<Guid>>>();*/

// 4. Регистрируем обработчики команд

builder.Services.AddTransient<IRequestHandler<DeleteUserCommand, Result>, DeleteUserHandler>();
builder.Services.AddTransient<IRequestHandler<GetCurrentUserQuery, IActionResult>, GetCurrentUserHandler>();
builder.Services.AddTransient<IRequestHandler<RegistrationCommand, IActionResult>, RegistrationCommandHandler>();
builder.Services.AddTransient<IRequestHandler<UpdateUserCommand, Result>, UpdateUserHandler>();
builder.Services.AddTransient<IRequestHandler<GetUserByIdQuery, Result<UserDataDto>>, GetUserByIdQueryHandler>();

// Регистрация MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(RegistrationCommandHandler).Assembly));

// Валидаторы
builder.Services.AddValidatorsFromAssemblyContaining<LoginQueryValidator>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Создание ролей при запуске
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    var roles = new[] { "Administrator", "Manager", "Employee", "Client" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");
app.UseStaticFiles();
app.MapControllers();

app.Run();