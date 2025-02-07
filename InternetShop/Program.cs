using InternetShop.Application.User;
using InternetShop.Database;
using InternetShop.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Исправление регистрации Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
   /* options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;
    options.User.RequireUniqueEmail = true;*/
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 2. Удалить дублирующую регистрацию Identity
// Удалить эти строки:
// builder.Services.AddIdentityCore<User>()
//     .AddEntityFrameworkStores<ApplicationDbContext>()
//     .AddSignInManager<SignInManager<User>>();

// 3. Исправление конфигурации JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);
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

// 4. Регистрация контекста БД
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("InternetShopDbContext")));

// 5. Регистрация MediatR
//builder.Services.AddMediatR(cfg =>
//{
//    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
//    //cfg.RegisterServicesFromAssemblies(typeof(LoginQueryHandler).Assembly);
//});

builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

// 6. Удалить явную регистрацию обработчиков
// Удалить эти строки:
/*builder.Services.AddScoped<LoginQueryHandler>();
builder.Services.AddScoped<RegistrationCommandHandler>();
builder.Services.AddScoped<CreateUserHandler>();
builder.Services.AddScoped<GetAllUsersHandler>();*/

// 7. Исправление регистрации сервисов
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, JWTTokenService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

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
app.UseAuthorization();
app.MapControllers();
app.Run();