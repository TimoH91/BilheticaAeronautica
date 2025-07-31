using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;

using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;

namespace BilheticaAeronauticaWeb.Services
{
    public class UserService : IUserService
    {
        private readonly IUserHelper _userHelper;
        private readonly ITicketRepository _ticketRepository;
        public UserService(IUserHelper userHelper, ITicketRepository ticketRepository)
        {
            _userHelper = userHelper;
            _ticketRepository = ticketRepository;
        }

        public async Task<bool> AllowUserDeletion(User user)
        {
            var ticket = await _ticketRepository.GetTicketsByUser(user);

            if (user.Email == "timothyharris04@gmail.com" || ticket != null)
            {
                return false;
            }

            return true;
            
        }
    }
}

