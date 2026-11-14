using System.Security.Claims;

namespace cometPOS.Services
{
    public static class UsuarioActualExtensions
    {
        public const string SuperAdmin = "SuperAdmin";

        public static int? EmpresaId(this ClaimsPrincipal user) =>
            int.TryParse(user.FindFirstValue("EmpresaId"), out var id) ? id : null;

        public static int? SucursalId(this ClaimsPrincipal user) =>
            int.TryParse(user.FindFirstValue("SucursalId"), out var id) ? id : null;

        public static bool EsSuperAdmin(this ClaimsPrincipal user) =>
            user.IsInRole(SuperAdmin);
    }
}
