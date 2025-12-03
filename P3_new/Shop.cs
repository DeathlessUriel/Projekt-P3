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
            _stock.Add("Włócznia", (() => new AdvancedWeapon("Włócznia", 6, 10, 1), 40));
            _stock.Add("Topór bojowy", (() => new AdvancedWeapon("Topór bojowy", 8, 8, 2), 55));
        }

        // ────────────────────────────────────────────────
        // Pomocnicze kolory
        // ────────────────────────────────────────────────
        private void CWrite(string text, ConsoleColor c)
        {
            Console.ForegroundColor = c;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private void CInline(string text, ConsoleColor c)
        {
            Console.ForegroundColor = c;
            Console.Write(text);
            Console.ResetColor();
        }

        // ────────────────────────────────────────────────
        // WEJŚCIE DO SKLEPU
        // ────────────────────────────────────────────────
        public void Enter(Player player)
        {
            CWrite("\n=== SKLEP ===", ConsoleColor.Yellow);
            CWrite("Witaj w sklepie podróżniku!", ConsoleColor.Cyan);
            Console.WriteLine("Oto dostępne przedmioty:\n");

            int i = 1;
            foreach (var kv in _stock)
            {
                CInline($"{i}. ", ConsoleColor.White);
                CInline(kv.Key, ConsoleColor.Yellow);
                CWrite($" — {kv.Value.Price} zł", ConsoleColor.DarkYellow);
                i++;
            }

            Console.WriteLine();
            CInline("Wybierz numer aby kupić (", ConsoleColor.White);
            CInline("0 = wyjście", ConsoleColor.DarkGray);
            CWrite("):", ConsoleColor.White);

            Console.Write("> ");
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                CWrite("Niepoprawny wybór.", ConsoleColor.Red);
                return;
            }

            if (choice <= 0 || choice > _stock.Count)
            {
                CWrite("Wychodzisz ze sklepu.", ConsoleColor.DarkYellow);
                return;
            }

            string itemKey = new List<string>(_stock.Keys)[choice - 1];
            var info = _stock[itemKey];

            // ────────────────────────────────────────────────
            // Sprawdzenie złota
            // ────────────────────────────────────────────────
            if (player.Gold < info.Price)
            {
                CWrite("Za mało złota!", ConsoleColor.Red);
                return;
            }

            var item = info.Create();

            if (player.Inventory.Add(item))
            {
                player.Gold -= info.Price;

                CInline("Kupiono ", ConsoleColor.Green);
                CInline(itemKey, ConsoleColor.Yellow);
                CWrite($" za {info.Price} zł!", ConsoleColor.DarkYellow);
            }
            else
            {
                CWrite("Ekwipunek jest pełny!", ConsoleColor.Red);
            }
        }
    }
}
