using System.Data;
using System.Threading.Tasks;
using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BilheticaAeronauticaWeb.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task CreateAsync(User user, string role, string password)
        {

            var result1 = await _userManager.CreateAsync(user, password);

            if (!result1.Succeeded)
            {
                throw new Exception(string.Join("; ", result1.Errors.Select(e => e.Description)));
            }

            var result2 = await _userManager.AddToRoleAsync(user, role);

            if (!result2.Succeeded)
            {
                throw new Exception(string.Join("; ", result2.Errors.Select(e => e.Description)));
            }
        }

        public async Task CreateRangeAsync(IEnumerable<User> users)
        {
            // UserManager doesn't have a bulk create, so do them one by one
            foreach (var user in users)
            {
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create user {user.UserName}: " + string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        public async Task DeleteAsync(User user)
        {
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task<bool> ExistAsync(string? id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<User> GetAll()
        {
            return _userManager.Users;
        }

        public async Task<List<User>> GetAllWithRoles()
        {
            var users = _userManager.Users.ToList(); // Materialize query
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Role = roles.FirstOrDefault() ?? "No Role";
            }
            return users;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task UpdateAsync(User user)
        {
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
