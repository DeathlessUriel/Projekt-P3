using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TextRpg
{
    public class LoadHandler
    {
        private const string SavePath = "save.json";

        // DTO — musi być identyczny jak w SaveHandler
        private class PlayerDto
        {
            public string Name { get; set; } = "";
            public int HP { get; set; }
            public int MaxHP { get; set; }
            public int BaseAttack { get; set; }
            public int Gold { get; set; }
            public int Level { get; set; }
            public int XP { get; set; }
            public List<ItemDto> Inventory { get; set; } = new();
            public string? EquippedName { get; set; }
        }

        private class ItemDto
        {
            public string Type { get; set; } = "";
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public int Damage { get; set; }
            public int CritChance { get; set; }
            public int RequiredLevel { get; set; }
            public int HealAmount { get; set; }
            public int GoldAmount { get; set; }
        }

        // ────────────────────────────────────────────
        // LOAD
        // ────────────────────────────────────────────
        public Player? Load()
        {
            if (!File.Exists(SavePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Brak zapisu gry.");
                Console.ResetColor();
                return null;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                var dto = JsonSerializer.Deserialize<PlayerDto>(json);

                if (dto == null)
                    return null;

                var p = new Player(dto.Name);

                typeof(Player).GetProperty("HP")?.SetValue(p, dto.HP);
                typeof(Player).GetProperty("MaxHP")?.SetValue(p, dto.MaxHP);
                typeof(Player).GetProperty("BaseAttack")?.SetValue(p, dto.BaseAttack);
                typeof(Player).GetProperty("Level")?.SetValue(p, dto.Level);
                typeof(Player).GetProperty("XP")?.SetValue(p, dto.XP);

                p.Gold = dto.Gold;

                foreach (var it in dto.Inventory)
                {
                    Item? item = it.Type switch
                    {
                        "AdvancedWeapon" => new AdvancedWeapon(it.Name, it.Damage, it.CritChance, it.RequiredLevel),
                        "Weapon"         => new Weapon(it.Name, it.Damage),
                        "HealthPotion"   => new HealthPotion() { HealAmount = it.HealAmount },
                        "ItemGold"       => new ItemGold(it.GoldAmount),
                        _                => null
                    };

                    if (item != null)
                        p.Inventory.Add(item);
                }

                // Wyposażenie
                if (!string.IsNullOrWhiteSpace(dto.EquippedName))
                {
                    foreach (var it in p.Inventory.Items)
                    {
                        if (it is Weapon w && w.Name == dto.EquippedName)
                        {
                            p.EquipWeapon(w);
                            break;
                        }
                    }
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Wczytano grę.");
                Console.ResetColor();

                return p;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Błąd wczytywania: {ex.Message}");
                Console.ResetColor();
                return null;
            }
        }
    }
}
