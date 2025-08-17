using BilheticaAeronautica.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Validations
{
    public interface IValidator
    {
        string NameError { get; set; }
        string EmailError { get; set; }
        string TelephoneError { get; set; }
        string PasswordError { get; set; }
        string PassengerTypeError { get; set; }

        string TicketClassError { get; set; }
        Task<bool> ValidateLogin(string name, string email,
                           string telephone, string password);

        Task<bool> ValidateRegister(string name, string surname,
            string username, string password);

        Task<bool> ValidateTicket(string name, string surname,
            string passengerType, string ticketClass);
    }
}
