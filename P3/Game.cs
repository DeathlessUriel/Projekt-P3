using System;
using System.Threading.Tasks;

namespace TextRpg
{
    public class Game
    {
        private Player _player;
        private readonly LootGenerator _lootGen = new LootGenerator();
        private readonly Random _rng = new Random();
        private bool _running = true;

        public async Task StartAsync()
        {
            Welcome();
            CreateOrLoadPlayer();
            while (_running && _player.IsAlive)
            {
                ShowStatus();
                Console.WriteLine("Wybierz kierunek: (L)ewo, (P)rawo, (O)dpočzynek, (S)klep, (Z)apisz, (W)yjdź");
                var c = Console.ReadLine()?.Trim().ToUpper();

                switch (c)
                {
                    case "L":
                    case "P":
                        var encounter = Encounter.Generate(_rng, _lootGen);
                        await HandleEncounterAsync(encounter);
                        break;

                    case "O":
                        Console.WriteLine("Odpoczywasz [+5 HP]");
                        _player.Heal(5);
                        break;

                    case "S":
                        new Shop().Enter(_player);
                        break;

                    case "Z":
                        SaveSystem.Save(_player);
                        break;

                    case "W":
                        Console.WriteLine("Na pewno wyjść? (T/N)");
                        if (Console.ReadLine()?.Trim().ToUpper() == "T")
                            _running = false;
                        break;

                    default:
                        Console.WriteLine("Niepoprawny wybór.");
                        break;
                }
            }

            Console.WriteLine("Koniec gry.");
        }

        private async Task HandleEncounterAsync(Encounter encounter)
        {
            Console.WriteLine(encounter.Description);

            if (encounter.Type == EncounterType.Enemy)
                await CombatAsync(encounter.Enemy!);

            else if (encounter.Type == EncounterType.Loot)
            {
                var item = encounter.Item!;
                Console.WriteLine($"Znalazłeś: {item.Name} - {item.Description}");
                Console.WriteLine("Zabrać? (T/N)");
                if (Console.ReadLine()?.Trim().ToUpper() == "T")
                    _player.AddItem(item);
            }

            await Task.Delay(250);
        }

        private async Task CombatAsync(Enemy enemy)
        {
            Console.WriteLine($"Walka z: {enemy.Name} (HP: {enemy.HP})");

            while (enemy.IsAlive && _player.IsAlive)
            {
                Console.WriteLine("Akcja: (A)tack, (U)żyj przedmiot, (P)roba ucieczki");
                var act = Console.ReadLine()?.Trim().ToUpper();

                if (act == "A")
                {
                    var dmg = _player.Attack();
                    enemy.TakeDamage(dmg);
                    Console.WriteLine($"Zadajesz {dmg}. HP wroga: {enemy.HP}");
                }
                else if (act == "U")
                {
                    if (_player.Inventory.Count == 0)
                    {
                        Console.WriteLine("Brak przedmiotów.");
                        continue;
                    }

                    Console.WriteLine("Wybierz przedmiot:");
                    for (int i = 0; i < _player.Inventory.Count; i++)
                        Console.WriteLine($"{i + 1}. {_player.Inventory.Items[i].Name}");

                    if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > _player.Inventory.Count)
                    {
                        Console.WriteLine("Zły wybór.");
                        continue;
                    }

                    var item = _player.Inventory.Items[idx - 1];

                    if (item is IConsumable consumable)
                    {
                        consumable.Consume(_player);

                        // usuń tylko jeśli zużyty
                        if (item is LimitedUseItem lu && lu.UsesLeft <= 0)
                            _player.Inventory.Remove(item);
                        else if (item is not LimitedUseItem)
                            _player.Inventory.Remove(item);
                    }
                    else
                        Console.WriteLine("Nie można użyć tego przedmiotu teraz.");
                }
                else if (act == "P")
                {
                    if (_rng.NextDouble() < 0.50)
                    {
                        Console.WriteLine("Udało się uciec!");
                        return;
                    }
                    Console.WriteLine("Nie udało się uciec.");
                }

                // WRÓG ATAKUJE
                if (enemy.IsAlive)
                {
                    // efekt trucizny goblina
                    if (enemy is Goblin g)
                        g.ApplyPoisonEffect(_player);

                    // boss może ogłuszyć
                    if (enemy is Boss boss && boss.TryStun())
                    {
                        Console.WriteLine("Boss OGŁUSZA cię, pomijasz turę!");
                        continue;
                    }

                    var ed = enemy.Attack();
                    _player.TakeDamage(ed);
                    Console.WriteLine($"{enemy.Name} zadaje {ed}. Twoje HP: {_player.HP}");
                }
            }

            if (!_player.IsAlive)
            {
                Console.WriteLine("Zginąłeś.");
                _running = false;
                return;
            }

            Console.WriteLine($"Pokonałeś {enemy.Name}!");
            _player.GainXP(5);
            _player.Gold += 5;

            var loot = _lootGen.GenerateLoot();
            Console.WriteLine($"Zdobyto przedmiot: {loot.Name}");
            _player.AddItem(loot);

            await Task.Delay(200);
        }

        private void CreateOrLoadPlayer()
        {
            Console.WriteLine("Wpisz imię bohatera lub L aby wczytać zapis:");
            var input = Console.ReadLine()?.Trim();

            if (input?.ToUpper() == "L")
            {
                var loaded = SaveSystem.Load();
                if (loaded != null)
                {
                    _player = loaded;
                    Console.WriteLine("Wczytano postać.");
                    return;
                }
                Console.WriteLine("Brak zapisu — tworzenie nowej postaci.");
            }

            Console.WriteLine("Podaj imię postaci:");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = "Bohater";

            _player = new Player(name.Trim());
            _player.AddItem(new AdvancedWeapon("Stary miecz", 3));
            _player.AddItem(new HealthPotion());
        }

        private void ShowStatus()
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine($"Gracz: {_player.Name}");
            Console.WriteLine($"HP: {_player.HP}/{_player.MaxHP} | Poziom: {_player.Level}  XP: {_player.XP}");
            Console.WriteLine($"Złoto: {_player.Gold}  Przedmioty: {_player.Inventory.Count}");
            Console.WriteLine("-----------------------------");
        }

        private void Welcome()
        {
            Console.WriteLine("=== TEKSTOWE RPG ===");
        }
    }
}
