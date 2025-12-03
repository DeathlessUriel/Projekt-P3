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
        public EncounterType Type { get; }
        public Enemy? Enemy { get; }
        public Item? Item { get; }
        public string Description { get; }

        private Encounter(EncounterType type, Enemy? enemy, Item? item, string description)
        {
            Type = type;
            Enemy = enemy;
            Item = item;
            Description = description;
        }

        public static Encounter Empty(string desc = "Pusty pokój.")
            => new Encounter(EncounterType.Empty, null, null, desc);

        public static Encounter WithEnemy(Enemy enemy, string desc)
            => new Encounter(EncounterType.Enemy, enemy, null, desc);

        public static Encounter WithLoot(Item item, string desc = "Znalazłeś coś.")
            => new Encounter(EncounterType.Loot, null, item, desc);
    }
}
