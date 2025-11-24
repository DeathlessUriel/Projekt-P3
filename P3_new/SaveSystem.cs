using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TextRpg
{
    public static class SaveSystem
    {
        private static string SavePath => "save.json";

        // ─────────────────────────────────────────────────────────
        // DTO — struktury pomocnicze do zapisu i wczytania
        // ─────────────────────────────────────────────────────────
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

        private class PlayerDto
        {
            public string Name { get; set; } = "";

            public int HP { get; set; }
            public int MaxHP { get; set; }
            public int BaseAttack { get; set; }

            public int Gold { get; set; }
            public int Level { get; set; }
            public int XP { get; set; }

            public List<ItemDto> Inventory { get; set; } = new List<ItemDto>();

            public string? EquippedName { get; set; }
        }

        // ─────────────────────────────────────────────────────────
        // SAVE
        // ─────────────────────────────────────────────────────────
        public static void Save(Player player)
        {
            var dto = new PlayerDto
            {
                Name = player.Name,
                HP = player.HP,
                MaxHP = player.MaxHP,
                BaseAttack = player.BaseAttack,
                Gold = player.Gold,
                Level = player.Level,
                XP = player.XP,

                EquippedName = player.EquippedWeapon?.Name
            };

            foreach (var it in player.Inventory.Items)
            {
                var d = new ItemDto()
                {
                    Name = it.Name,
                    Description = it.Description
                };

                switch (it)
                {
                    case AdvancedWeapon aw:
                        d.Type = "AdvancedWeapon";
                        d.Damage = aw.Damage;
                        d.CritChance = aw.CritChance;
                        d.RequiredLevel = aw.RequiredLevel;
                        break;

                    case Weapon w:
                        d.Type = "Weapon";
                        d.Damage = w.Damage;
                        break;

                    case HealthPotion hp:
                        d.Type = "HealthPotion";
                        d.HealAmount = hp.HealAmount;
                        break;

                    case ItemGold g:
                        d.Type = "ItemGold";
                        d.GoldAmount = g.Amount;
                        break;

                    default:
                        d.Type = "Unknown";
                        break;
                }

                dto.Inventory.Add(d);
            }

            var options = new JsonSerializerOptions() { WriteIndented = true };
            var json = JsonSerializer.Serialize(dto, options);
            File.WriteAllText(SavePath, json);

            Console.WriteLine("Zapisano grę.");
        }

        // ─────────────────────────────────────────────────────────
        // LOAD
        // ─────────────────────────────────────────────────────────
        public static Player? Load()
        {
            if (!File.Exists(SavePath))
            {
                Console.WriteLine("Brak zapisu gry.");
                return null;
            }

            try
            {
                var json = File.ReadAllText(SavePath);
                var dto = JsonSerializer.Deserialize<PlayerDto>(json);

                if (dto == null)
                    return null;

                // Stwórz gracza
                var player = new Player(dto.Name);

                // ustaw staty prywatne refleksją
                var t = typeof(Player);

                t.GetProperty("HP")?.SetValue(player, dto.HP);
                t.GetProperty("MaxHP")?.SetValue(player, dto.MaxHP);
                t.GetProperty("BaseAttack")?.SetValue(player, dto.BaseAttack);
                t.GetProperty("Level")?.SetValue(player, dto.Level);
                t.GetProperty("XP")?.SetValue(player, dto.XP);

                player.Gold = dto.Gold;

                // ładujemy inventory
                foreach (var it in dto.Inventory)
                {
                    Item? obj = it.Type switch
                    {
                        "AdvancedWeapon" => new AdvancedWeapon(it.Name, it.Damage, it.CritChance, it.RequiredLevel),
                        "Weapon"         => new Weapon(it.Name, it.Damage),
                        "HealthPotion"   => new HealthPotion() { HealAmount = it.HealAmount },
                        "ItemGold"       => new ItemGold(it.GoldAmount),
                        _                => null
                    };

                    if (obj != null)
                        player.Inventory.Add(obj);
                }

                // wyposażyć broń jeśli jest znana
                if (!string.IsNullOrWhiteSpace(dto.EquippedName))
                {
                    foreach (var it in player.Inventory.Items)
                    {
                        if (it is Weapon w && w.Name == dto.EquippedName)
                        {
                            player.EquipWeapon(w);
                            break;
                        }
                    }
                }

                Console.WriteLine("Wczytano grę.");
                return player;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd odczytu zapisu: " + ex.Message);
                return null;
            }
        }
    }
}
