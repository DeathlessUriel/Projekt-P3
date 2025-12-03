using System;
using System.Threading.Tasks;

namespace TextRpg
{
    public class Game
    {
        private Player _player = null!;
        private bool _running = true;

        private readonly LootGenerator _lootGenerator = new LootGenerator();
        private readonly Shop _shop = new Shop();

        private IEncounterGenerator _encounterGenerator;
        private EnemyFactory _enemyFactory;

        // ────────────────────────────────────────────────
        // Pomocnicza metoda koloru
        // ────────────────────────────────────────────────
        private void CWrite(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private void CWriteInline(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        // ────────────────────────────────────────────────
        // START GRY
        // ────────────────────────────────────────────────
        public async Task StartAsync()
        {
            CWrite("=== Witaj w grze RPG ===", ConsoleColor.Cyan);
            Console.Write("1: Nowa gra  |  2: Wczytaj zapis\n> ");

            string? mode = Console.ReadLine();

            if (mode == "2")
            {
                var loaded = SaveManager.Load();
                if (loaded != null)
                {
                    _player = loaded;
                    CWrite($"\nWitaj ponownie, {_player.Name}!", ConsoleColor.Cyan);
                }
            }

            if (_player == null)
            {
                Console.Write("\nPodaj swoje imię: ");
                var name = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(name) || !Utils.ValidateName(name))
                    name = "Bohater";

                _player = new Player(name);

                CWrite($"\nWitaj, {_player.Name}!", ConsoleColor.Cyan);

                _player.AddItem(new Weapon("Kij bojowy", 1));
                _player.AddItem(new Weapon("Krótki miecz", 3));
                _player.AddItem(new AdvancedWeapon("Włócznia", 5, 10, 1));
                _player.Gold = 10;
            }

            await MainLoop();
        }

        // ────────────────────────────────────────────────
        // MAIN MENU
        // ────────────────────────────────────────────────
        private async Task MainLoop()
        {
            while (_running)
            {
                CWrite("\n=== MENU ===", ConsoleColor.Yellow);
                Console.WriteLine("1. Wyrusz na przygodę");
                Console.WriteLine("2. Statystyki");
                Console.WriteLine("3. Ekwipunek");
                Console.WriteLine("4. Sklep");
                Console.WriteLine("5. Zapisz grę");
                Console.WriteLine("6. Zakończ grę");
                Console.Write("> ");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1": Explore(); break;
                    case "2": ShowStats(); break;
                    case "3": InventoryMenu(); break;
                    case "4": _shop.Enter(_player); break;
                    case "5": SaveManager.Save(_player); break;
                    case "6": _running = false; break;
                    default:
                        CWrite("Nieznana komenda.", ConsoleColor.Red);
                        break;
                }

                await Task.Delay(50);
            }
        }

        // ────────────────────────────────────────────────
        // STATYSTYKI
        // ────────────────────────────────────────────────
        private void ShowStats()
        {
            CWrite("\n=== STATYSTYKI ===", ConsoleColor.Blue);
            CWriteInline("Imię:     ", ConsoleColor.DarkBlue); Console.WriteLine(_player.Name);
            CWriteInline("HP:       ", ConsoleColor.DarkBlue); Console.WriteLine($"{_player.HP}/{_player.MaxHP}");
            CWriteInline("Poziom:   ", ConsoleColor.DarkBlue); Console.WriteLine(_player.Level);
            CWriteInline("XP:       ", ConsoleColor.DarkBlue); Console.WriteLine(_player.XP);
            CWriteInline("Złoto:    ", ConsoleColor.DarkYellow); Console.WriteLine(_player.Gold);
            CWriteInline("Atak:     ", ConsoleColor.DarkBlue); Console.WriteLine(_player.BaseAttack);
            CWriteInline("Broń:     ", ConsoleColor.DarkBlue); Console.WriteLine(_player.EquippedWeapon?.Name ?? "brak");
        }

        // ────────────────────────────────────────────────
        // EKWIPUNEK
        // ────────────────────────────────────────────────
        private void InventoryMenu()
        {
            while (true)
            {
                CWrite("\n=== EKWIPUNEK ===", ConsoleColor.Yellow);

                if (_player.Inventory.Count == 0)
                {
                    CWrite("Ekwipunek jest pusty.", ConsoleColor.DarkYellow);
                    return;
                }

                for (int i = 0; i < _player.Inventory.Count; i++)
                {
                    Item item = _player.Inventory.Items[i];

                    CWriteInline($"{i + 1}. ", ConsoleColor.White);
                    CWriteInline(item.Name, ConsoleColor.Yellow);

                    if (item is Weapon w)
                        Console.Write($" (+{w.Damage} DMG)");

                    if (item is AdvancedWeapon aw)
                        Console.Write($" [ADV {aw.Damage} DMG, {aw.CritChance}% crit]");

                    if (item is HealthPotion hp)
                        Console.Write($" [HEAL {hp.HealAmount}]");

                    if (item is ItemGold gold)
                        Console.Write($" [GOLD {gold.Amount}]");

                    Console.WriteLine();
                }

                Console.WriteLine("\n1 = użyj / wyposaż  |  2 = wyrzuć  |  0 = powrót");
                Console.Write("> ");

                if (!int.TryParse(Console.ReadLine(), out int choice) || choice == 0)
                    return;

                int index = choice - 1;
                Item? it = _player.Inventory.GetAt(index);

                if (it == null)
                {
                    CWrite("Niepoprawny numer.", ConsoleColor.Red);
                    continue;
                }

                Console.Write("Akcja (1=użyj/wyposaż, 2=wyrzuć): ");
                string? action = Console.ReadLine();

                if (action == "1")
                {
                    if (it is Weapon weapon)
                    {
                        _player.EquipWeapon(weapon);
                    }
                    else if (it is HealthPotion hp)
                    {
                        hp.Consume(_player);
                        _player.RemoveItem(hp);
                        CWrite("Użyto mikstury!", ConsoleColor.Green);
                    }
                }
                else if (action == "2")
                {
                    _player.RemoveItem(it);
                    CWrite("Przedmiot usunięty.", ConsoleColor.DarkYellow);
                }
            }
        }

        // ────────────────────────────────────────────────
        // EKSPLORACJA
        // ────────────────────────────────────────────────
        private void Explore()
        {
            _enemyFactory = new EnemyFactory();

// rejestracja przeciwników z wagami
_enemyFactory.Register(() => new Goblin(), 0.40);
_enemyFactory.Register(() => new Wolf(),   0.25);
_enemyFactory.Register(() => new Bandit(), 0.20);
_enemyFactory.Register(() => new Orc(),    0.15);

_encounterGenerator = new EncounterGenerator(_enemyFactory, _lootGenerator);

            CWrite("\nWyruszasz na eksplorację...", ConsoleColor.Cyan);
            

           var encounter = _encounterGenerator.Generate();

            CWrite(encounter.Description, ConsoleColor.White);

            if (encounter.Type == EncounterType.Empty)
                return;

            if (encounter.Type == EncounterType.Loot && encounter.Item != null)
            {
                CWrite($"Znalazłeś: {encounter.Item.Name}", ConsoleColor.Yellow);

                if (encounter.Item is ItemGold g)
                {
                    _player.Gold += g.Amount;
                    CWrite($"+{g.Amount} złota!", ConsoleColor.DarkYellow);
                }
                else
                {
                    _player.AddItem(encounter.Item);
                }
                return;
            }

            if (encounter.Type == EncounterType.Enemy && encounter.Enemy != null)
                Fight(encounter.Enemy);
        }

        // ────────────────────────────────────────────────
        // WALKA
        // ────────────────────────────────────────────────
        private void Fight(Enemy enemy)
        {
            CWrite($"\nSpotykasz przeciwnika: {enemy.Name}!", ConsoleColor.Red);

            while (enemy.IsAlive && _player.IsAlive)
            {
                CWriteInline("\nTwój HP: ", ConsoleColor.Blue);
                Console.WriteLine($"{_player.HP}/{_player.MaxHP}");

                CWriteInline($"{enemy.Name} HP: ", ConsoleColor.Red);
                Console.WriteLine(enemy.HP);

                Console.WriteLine("1. Atak");
                Console.WriteLine("2. Ucieczka");
                Console.Write("> ");

                string? command = Console.ReadLine();

                if (command == "1")
                {
                    int dmg = _player.Attack();
                    enemy.TakeDamage(dmg);
                    CWrite($"Zadajesz {dmg} obrażeń!", ConsoleColor.Green);

                    if (!enemy.IsAlive)
                    {
                        CWrite("\nPokonałeś przeciwnika!", ConsoleColor.Green);
                        _player.GainXP(6);
                        _player.Gold += 3;
                        CWrite("+3 złota!", ConsoleColor.DarkYellow);
                        return;
                    }

                    int enemyDmg = enemy.Attack();
                    _player.TakeDamage(enemyDmg);
                    CWrite($"{enemy.Name} zadaje {enemyDmg} obrażeń!", ConsoleColor.DarkRed);

                    if (enemy is Goblin gob)
                        gob.ApplyPoisonEffect(_player);

                    if (!_player.IsAlive)
                    {
                        CWrite("\nZginąłeś... Koniec gry.", ConsoleColor.DarkRed);
                        _running = false;
                        return;
                    }
                }
                else if (command == "2")
                {
                    CWrite("Uciekasz z pola walki!", ConsoleColor.DarkYellow);
                    return;
                }
            }
        }
    }
}
