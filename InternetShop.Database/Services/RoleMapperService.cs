using InternetShop.Domain;

namespace InternetShop.Database.Services
{
    public static class RoleMapperService
    {
        public static string ToName(RoleEnum role) => role switch
        {
            RoleEnum.Admin => "Admin",
            RoleEnum.Manager => "Manager",
            RoleEnum.Employee => "Employee",
            RoleEnum.Client => "Client",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };

        public static RoleEnum FromString(string roleName) => roleName.ToLower() switch
        {
            "admin" => RoleEnum.Admin,
            "manager" => RoleEnum.Manager,
            "employee" => RoleEnum.Employee,
            "client" => RoleEnum.Client,
            _ => RoleEnum.Client
        };
    }
}
