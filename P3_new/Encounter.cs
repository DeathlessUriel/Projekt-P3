using System;

namespace TextRpg
{
    public enum EncounterType
    {
        Empty,
        Enemy,
        Loot
    }

    public class Encounter
    {
        public EncounterType Type { get; set; }
        public Enemy? Enemy { get; set; }
        public Item? Item { get; set; }
        public string Description { get; set; } = "";

        public static Encounter Generate(LootGenerator lg)
        {
            var rng = Utils.RNG;
            double roll = rng.NextDouble();

            // ──────────────────────────────────────────────
            // 50% szans na przeciwnika
            // ──────────────────────────────────────────────
            if (roll < 0.5)
            {
                double enemyRoll = rng.NextDouble();
                Enemy e;

                if (enemyRoll < 0.40)         e = new Goblin();
                else if (enemyRoll < 0.65)    e = new Wolf();
                else if (enemyRoll < 0.85)    e = new Bandit();
                else                          e = new Orc();

                return new Encounter
                {
                    Type = EncounterType.Enemy,
                    Enemy = e,
                    Description = $"Spotykasz {e.Name}!"
                };
            }

            // ──────────────────────────────────────────────
            // 30% szans na loot
            // ──────────────────────────────────────────────
            if (roll < 0.8)
            {
                var item = lg.GenerateLoot();

                return new Encounter
                {
                    Type = EncounterType.Loot,
                    Item = item,
                    Description = "Znalazłeś coś w pomieszczeniu."
                };
            }

            // ──────────────────────────────────────────────
            // 20% — puste pomieszczenie
            // ──────────────────────────────────────────────
            return new Encounter
            {
                Type = EncounterType.Empty,
                Description = "Pusty pokój."
            };
        }
    }
}
