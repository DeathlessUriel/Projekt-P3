using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TextRpg
{
    public class SaveHandler
    {
        private const string SavePath = "save.json";

        // DTO
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
        // SAVE
        // ────────────────────────────────────────────
        public void Save(Player player)
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
                var d = new ItemDto
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
                }

                dto.Inventory.Add(d);
            }

            string json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SavePath, json);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Zapisano grę.");
            Console.ResetColor();
        }
    }
}
