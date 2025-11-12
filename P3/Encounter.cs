using System;

namespace TextRpg
{
    public enum EncounterType { Empty, Enemy, Loot }

    public class Encounter
    {
        public EncounterType Type { get; set; }
        public Enemy? Enemy { get; set; }
        public Item? Item { get; set; }
        public string Description { get; set; } = "";

        public static Encounter Generate(Random rng, LootGenerator lg)
        {
            var roll = rng.NextDouble();
            if (roll < 0.5)
            {
                var e = new Enemy("Dziki wilk", 12 + rng.Next(0,6), 2 + rng.Next(0,3));
                return new Encounter { Type = EncounterType.Enemy, Enemy = e, Description = "Spotykasz wroga!" };
            }
            else if (roll < 0.8)
            {
                var item = lg.GenerateLoot();
                return new Encounter { Type = EncounterType.Loot, Item = item, Description = "Znalazłeś coś w pomieszczeniu." };
            }
            else return new Encounter { Type = EncounterType.Empty, Description = "Pusty pokój." };
        }
    }
}