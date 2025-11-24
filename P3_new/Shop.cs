using System;
using System.Collections.Generic;

namespace TextRpg
{
    public class Shop
    {
        private readonly Dictionary<string, (Func<Item> Create, int Price)> _stock
            = new Dictionary<string, (Func<Item>, int)>();

        public Shop()
        {
            _stock.Add("Mikstura zdrowia", (() => new HealthPotion(), 15));
            _stock.Add("Krótki miecz", (() => new Weapon("Krótki miecz", 4), 25));
            _stock.Add("Włócznia", (() => new AdvancedWeapon("Włócznia", 6, critChance: 10, reqLevel: 1), 40));
            _stock.Add("Topór bojowy", (() => new AdvancedWeapon("Topór bojowy", 8, critChance: 8, reqLevel: 2), 55));
        }

        public void Enter(Player player)
        {
            Console.WriteLine("Witaj w sklepie! Dostępne przedmioty:\n");

            int i = 1;
            foreach (var kv in _stock)
            {
                Console.WriteLine($"{i}. {kv.Key} - {kv.Value.Price} zł");
                i++;
            }

            Console.WriteLine("\nWybierz numer aby kupić (0 = wyjście):");

            Console.Write("> ");
            if (!int.TryParse(Console.ReadLine(), out int choice))
                return;

            if (choice <= 0 || choice > _stock.Count)
                return;

            string itemKey = new List<string>(_stock.Keys)[choice - 1];
            var info = _stock[itemKey];

            if (player.Gold < info.Price)
            {
                Console.WriteLine("Za mało złota.");
                return;
            }

            var item = info.Create();

            if (player.Inventory.Add(item))
            {
                player.Gold -= info.Price;
                Console.WriteLine($"Kupiono {itemKey}.");
            }
        }
    }
}
