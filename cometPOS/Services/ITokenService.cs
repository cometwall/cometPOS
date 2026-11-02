using cometPOS.Models;
namespace cometPOS.Services
{
    public interface ITokenService
    {
        string GenerarToken(Usuario usuario);
        (string Token, string Hash, DateTime Expira) GenerarRefreshToken();
        string HashearToken(string token);
    }
}
