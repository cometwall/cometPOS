using cometPOS.Models;
namespace cometPOS.Services
{
    public interface ITokenService
    {
        string GenerarToken(Usuario usuario);
    }
}
