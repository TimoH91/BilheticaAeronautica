using BilheticaAeronautica.Mobile.Models;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace BilheticaAeronautica.Mobile.Services
{
    public class BasketService : IBasketService
    {

        private const string BasketKey = "Basket";

        private readonly ObservableCollection<ShoppingBasketTicket> _items = new();
        public ObservableCollection<ShoppingBasketTicket> Items => _items;

        public int InfantSeatId { get; set; }

        public int ResponsibleAdultId { get; set; }

        public BasketService()
        {
            var json = Preferences.Get(BasketKey, "[]");
            var items = JsonSerializer.Deserialize<List<ShoppingBasketTicket>>(json) ?? new();
            foreach (var item in items)
                _items.Add(item);
        }

        public void Add(ShoppingBasketTicket ticket)
        {
            _items.Add(ticket);
            Save();
        }

        public void Remove(ShoppingBasketTicket ticket)
        {
            _items.Remove(ticket);
            Save();

        }

        public void Clear()
        {
            _items.Clear();
            Save();
        }

        private void Save()
        {
            var json = JsonSerializer.Serialize(_items);
            Preferences.Set(BasketKey, json);
        }

    }
}
