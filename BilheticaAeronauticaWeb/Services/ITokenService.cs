using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);

        Task<string> GenerateEmailConfirmationLinkAsync(User user);
            
    }
}
