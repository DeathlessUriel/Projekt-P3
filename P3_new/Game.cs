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

        // ────────────────────────────────────────────────
        // START GRY
        // ────────────────────────────────────────────────
        public async Task StartAsync()
        {
            Console.WriteLine("=== Witaj w grze RPG ===");
            Console.Write("1: Nowa gra  |  2: Wczytaj zapis\n> ");

            string? mode = Console.ReadLine();

            // Wczytywanie gry
            if (mode == "2")
            {
                var loaded = SaveSystem.Load();
                if (loaded != null)
                {
                    _player = loaded;
                    Console.WriteLine($"\nWitaj ponownie, {_player.Name}!");
                }
            }

            // Tworzenie nowej gry
            if (_player == null)
            {
                Console.Write("\nPodaj swoje imię: ");
                var name = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(name) || !Utils.ValidateName(name))
                    name = "Bohater";

                _player = new Player(name);

                Console.WriteLine($"\nWitaj, {_player.Name}!");

                // przedmioty początkowe
                _player.AddItem(new Weapon("Kij bojowy", 1));
                _player.AddItem(new Weapon("Krótki miecz", 3));
                _player.AddItem(new AdvancedWeapon("Włócznia", 5, critChance: 10, reqLevel: 1));

                _player.Gold = 10;
            }

            await MainLoop();
        }

        // ────────────────────────────────────────────────
        // GŁÓWNA PĘTLA GRY / MENU
        // ────────────────────────────────────────────────
        private async Task MainLoop()
        {
            while (_running)
            {
                Console.WriteLine("\n=== MENU ===");
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
                    case "5": SaveSystem.Save(_player); break;
                    case "6": _running = false; break;

                    default:
                        Console.WriteLine("Nieznana komenda.");
                        break;
                }

                await Task.Delay(50);
            }
        }

        // ────────────────────────────────────────────────
        // WYŚWIETLANIE STATYSTYK
        // ────────────────────────────────────────────────
        private void ShowStats()
        {
            Console.WriteLine("\n=== STATYSTYKI ===");
            Console.WriteLine($"Imię:     {_player.Name}");
            Console.WriteLine($"HP:       {_player.HP}/{_player.MaxHP}");
            Console.WriteLine($"Poziom:   {_player.Level}");
            Console.WriteLine($"XP:       {_player.XP}");
            Console.WriteLine($"Złoto:    {_player.Gold}");
            Console.WriteLine($"Atak:     {_player.BaseAttack}");
            Console.WriteLine($"Broń:     {_player.EquippedWeapon?.Name ?? "brak"}");
        }

        // ────────────────────────────────────────────────
        // MENU EKWIPUNKU
        // ────────────────────────────────────────────────
        private void InventoryMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== EKWIPUNEK ===");

                if (_player.Inventory.Count == 0)
                {
                    Console.WriteLine("Ekwipunek jest pusty.");
                    return;
                }

                for (int i = 0; i < _player.Inventory.Count; i++)
                {
                    Item it = _player.Inventory.Items[i];
                    Console.Write($"{i + 1}. {it.Name}");

                    if (it is Weapon w)
                        Console.Write($" (+{w.Damage} DMG)");

                    if (it is AdvancedWeapon aw)
                        Console.Write($" [ADV: {aw.Damage} DMG, {aw.CritChance}% crit]");

                    if (it is HealthPotion hp)
                        Console.Write($" [HEAL: {hp.HealAmount}]");

                    if (it is ItemGold gold)
                        Console.Write($" [GOLD: {gold.Amount}]");

                    Console.WriteLine();
                }

                Console.WriteLine("\nWybierz numer przedmiotu:");
                Console.WriteLine("1 = użyj / wyposaż  |  2 = wyrzuć  |  0 = powrót");
                Console.Write("> ");

                string? input = Console.ReadLine();
                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Niepoprawny wybór.");
                    continue;
                }

                if (choice == 0)
                    return;

                int index = choice - 1;
                Item? item = _player.Inventory.GetAt(index);

                if (item == null)
                {
                    Console.WriteLine("Niepoprawny numer.");
                    continue;
                }

                Console.Write("Akcja (1 = użyj/wyposaż, 2 = wyrzuć, 0 = anuluj): ");
                string? action = Console.ReadLine();

                if (action == "0")
                    continue;

                if (action == "1")
                {
                    if (item is Weapon weapon)
                    {
                        _player.EquipWeapon(weapon);
                    }
                    else if (item is HealthPotion hp)
                    {
                        hp.Consume(_player);
                        _player.RemoveItem(hp); // mikstura znika po użyciu
                    }
                    else
                    {
                        Console.WriteLine("Tego przedmiotu nie można użyć.");
                    }
                }
                else if (action == "2")
                {
                    _player.RemoveItem(item);
                    Console.WriteLine("Przedmiot został wyrzucony.");
                }
            }
        }

        // ────────────────────────────────────────────────
        // EKSPLORACJA
        // ────────────────────────────────────────────────
        private void Explore()
        {
            Console.WriteLine("\nWyruszasz na eksplorację...");

            Encounter encounter = Encounter.Generate(_lootGenerator);
            Console.WriteLine(encounter.Description);

            // Pusty pokój
            if (encounter.Type == EncounterType.Empty)
                return;

            // Loot
            if (encounter.Type == EncounterType.Loot && encounter.Item != null)
            {
                Console.WriteLine($"Znalazłeś: {encounter.Item.Name}");

                if (encounter.Item is ItemGold gold)
                {
                    _player.Gold += gold.Amount;
                    Console.WriteLine($"Otrzymujesz {gold.Amount} złota!");
                }
                else
                {
                    _player.AddItem(encounter.Item);
                }
                return;
            }

            // Walka
            if (encounter.Type == EncounterType.Enemy && encounter.Enemy != null)
            {
                Fight(encounter.Enemy);
            }
        }

        // ────────────────────────────────────────────────
        // WALKA
        // ────────────────────────────────────────────────
        private void Fight(Enemy enemy)
        {
            Console.WriteLine($"\nSpotykasz przeciwnika: {enemy.Name}! (HP: {enemy.HP})");

            while (enemy.IsAlive && _player.IsAlive)
            {
                Console.WriteLine($"\nTwój HP: {_player.HP}/{_player.MaxHP}");
                Console.WriteLine($"{enemy.Name} HP: {enemy.HP}");

                Console.WriteLine("1. Atak");
                Console.WriteLine("2. Ucieczka");
                Console.Write("> ");

                string? command = Console.ReadLine();

                if (command == "1")
                {
                    int dmg = _player.Attack();
                    enemy.TakeDamage(dmg);
                    Console.WriteLine($"\nZadajesz {dmg} obrażeń!");

                    if (!enemy.IsAlive)
                    {
                        Console.WriteLine("\nPokonałeś przeciwnika!");
                        _player.GainXP(6);
                        _player.Gold += 3;
                        Console.WriteLine("Zdobywasz 3 złota!");
                        return;
                    }

                    int enemyDmg = enemy.Attack();
                    _player.TakeDamage(enemyDmg);
                    Console.WriteLine($"{enemy.Name} zadaje Ci {enemyDmg} obrażeń!");

                    if (enemy is Goblin gob)
                        gob.ApplyPoisonEffect(_player);

                    if (!_player.IsAlive)
                    {
                        Console.WriteLine("\nZginąłeś... Koniec gry.");
                        _running = false;
                        return;
                    }
                }
                else if (command == "2")
                {
                    Console.WriteLine("Uciekasz z pola walki!");
                    return;
                }
                else
                {
                    Console.WriteLine("Nieznana akcja.");
                }
            }
        }
    }
}
