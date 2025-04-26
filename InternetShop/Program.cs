using FluentValidation;
using InternetShop.Application.BusinessLogic.Cart;
using InternetShop.Application.BusinessLogic.Product;
using InternetShop.Application.BusinessLogic.User;
using InternetShop.Application.BusinessLogic.User.Login;
using InternetShop.Application.BusinessLogic.User.Registration;
using InternetShop.Database;
using InternetShop.Database.Models.Authentication;
using InternetShop.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


// Регистрация Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<UserDbContext>()
.AddDefaultTokenProviders();

// Конфигурация JWT
var jwtKey = builder.Configuration["Jwt:Key"] ??
    throw new ArgumentNullException("Jwt:Key is not configured");
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
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
var pgConnectionString = builder.Configuration.GetConnectionString("InternetShopDbContext") ??
    throw new ArgumentNullException("InternetShopDbContext connection string is not configured");
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(pgConnectionString));

// Регистрация сервисов
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<ITokenService, JWTTokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

// Регистрация MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(RegistrationCommandHandler).Assembly));


// Валидаторы
builder.Services.AddValidatorsFromAssemblyContaining<LoginQueryValidator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors("AllowAll");
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();