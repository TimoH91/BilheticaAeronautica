using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Services
{
    public interface IUserService
    {
        Task<bool> AllowUserDeletion(User user);
    }
}
