using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRpg
{
    public class LootGenerator
    {
        private readonly Random _rng = new Random();
        private readonly List<Func<Item>> _pool;

        public LootGenerator()
        {
            _pool = new List<Func<Item>>
            {
                () => new Weapon("Krótki miecz", 4),
                () => new Weapon("Włócznia", 5),
                () => new HealthPotion(),
                () => new ItemGold(10)
            };
        }

        public Item GenerateLoot() => _pool[_rng.Next(_pool.Count)]();
    }

    [Serializable]
    public class ItemGold : Item
    {
        public int Amount { get; set; }
        public ItemGold(int amount) : base($"Złoto x{amount}", "Monety") { Amount = amount; }
    }
}