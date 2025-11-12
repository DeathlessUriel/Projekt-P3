using System;
using System.Collections.Generic;

namespace TextRpg
{
    public class Shop
    {
        private readonly Dictionary<string, (Func<Item> Create, int Price)> _stock = new Dictionary<string, (Func<Item>, int)>();

        public Shop()
        {
            _stock.Add("Mikstura zdrowia", (() => new HealthPotion(), 15));
            _stock.Add("Krótki miecz", (() => new AdvancedWeapon("Krótki miecz", 4, critChance: 5, reqLevel:1), 25));
            _stock.Add("Włócznia", (() => new AdvancedWeapon("Włócznia", 6, critChance: 10, reqLevel:2), 40));
        }

        public void Enter(Player player)
        {
            Console.WriteLine("Witaj w sklepie! Dostępne przedmioty:");
            int i = 1;
            foreach (var kv in _stock)
            {
                Console.WriteLine($"{i}. {kv.Key} - {kv.Value.Price} zł");
                i++;
            }
            Console.WriteLine("Wybierz numer aby kupić (0 wyjście):");
            if (!int.TryParse(Console.ReadLine(), out int choice)) return;
            if (choice <= 0) return;
            if (choice > _stock.Count) return;
            var itemKey = new List<string>(_stock.Keys)[choice - 1];
            var info = _stock[itemKey];
            if (player.Gold < info.Price) { Console.WriteLine("Za mało złota."); return; }
            var it = info.Create();
            if (player.Inventory.Add(it))
            {
                player.Gold -= info.Price;
                Console.WriteLine($"Kupiono {itemKey}.");
            }
        }
    }
}
