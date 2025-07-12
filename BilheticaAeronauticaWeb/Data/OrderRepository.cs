using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

namespace BilheticaAeronauticaWeb.Data
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;


        public OrderRepository(DataContext context, IUserHelper userHelper, IConverterHelper converterHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                return null;
            }

            if (await _userHelper.IsUserInRoleAsync(user, "Customer"))
            {
                return await _context.Orders.
                    Include(o => o.User).
                    Include(o => o.Tickets).
                    Where(o => o.User == user)
                    .ToListAsync();
            }

            return null;
        }

        //public async Task<IEnumerable<Order>> GetOrdersByUserAsync(string userName)
        //{
        //    var orders = new List<Order>();
        //    var user = await _userHelper.GetUserByEmailAsync(userName);

        //    if (user == null)
        //    {
        //        return orders;
        //    }

        //    if (await _userHelper.IsUserInRoleAsync(user, "Customer"))
        //    {
        //        return _context.Orders.
        //            Include(o => o.User).
        //            Include(o => o.Tickets).
        //            Where(o => o.User == user);
        //    }

        //    return null;
        //}



        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.Tickets)
                .Include(o => o.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

    }
}
