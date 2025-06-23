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
            // Note: Your user ID is string (GUID), so maybe this int id doesn't fit, 
            // you might want string id here instead
            // But to keep your signature:
            // Try to convert int id to string or change method signature to string id

            // Here's a rough approach (if you changed method signature to string id):
            // var user = await _userManager.FindByIdAsync(id);
            // return user != null;

            // Or throw NotImplemented if id is int and you don't have conversion
            throw new NotImplementedException("ExistAsync should use string id, not int");
        }

        public IQueryable<User> GetAll()
        {
            // UserManager does not have IQueryable access directly.
            // You can expose _userManager.Users IQueryable
            return _userManager.Users;
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
