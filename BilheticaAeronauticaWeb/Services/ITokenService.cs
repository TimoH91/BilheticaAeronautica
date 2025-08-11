using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Services
{
    public interface ITokenService
    {
        public string GenerateToken(User user);
    }
}
