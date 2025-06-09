using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Identity;

namespace BilheticaAeronauticaWeb.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email); 

        Task<IdentityResult> AddUserAsync(User user, string password);
    }
}
