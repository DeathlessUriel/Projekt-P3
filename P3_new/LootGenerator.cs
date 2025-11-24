using System;
using System.Collections.Generic;

namespace TextRpg
{
    public class LootGenerator
    {
        private readonly List<Func<Item>> _pool;

        public LootGenerator()
        {
            _pool = new List<Func<Item>>
            {
                () => new Weapon("Krótki miecz", 4),
                () => new AdvancedWeapon("Włócznia", 6, critChance: 10, reqLevel: 1),
                () => new AdvancedWeapon("Topór bojowy", 8, critChance: 8, reqLevel: 2),
                () => new HealthPotion(),
                () => new ItemGold(10)
            };
        }

        public Item GenerateLoot()
        {
            return _pool[Utils.RNG.Next(_pool.Count)]();
        }
    }
}
