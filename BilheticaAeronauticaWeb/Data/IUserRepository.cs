using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Data
{
    public interface IUserRepository
    {
        IQueryable<User> GetAll();

        Task<User> GetByIdAsync(string id);

        Task CreateAsync(User user, string role, string password);

        Task UpdateAsync(User user);

        Task DeleteAsync(User user);

        Task<bool> ExistAsync(string? id);

        Task CreateRangeAsync(IEnumerable<User> users);
    }
}
