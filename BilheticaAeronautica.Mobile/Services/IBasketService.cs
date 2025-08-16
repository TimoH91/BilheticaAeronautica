using BilheticaAeronautica.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Services
{

    public interface IBasketService
    {
        ObservableCollection<ShoppingBasketTicket> Items { get; }

        void Add(ShoppingBasketTicket ticket);

        void Remove(ShoppingBasketTicket ticket);

        void Clear();

    }

    
}
