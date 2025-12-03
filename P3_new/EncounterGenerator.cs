using System;

namespace TextRpg
{
    public class EncounterGenerator : IEncounterGenerator
    {
        private readonly EnemyFactory _enemyFactory;
        private readonly LootGenerator _loot;

        /// <summary>
        /// losy: 50% wróg, 30% loot, 20% pusto
        /// </summary>
        private readonly double _enemyChance = 0.5;
        private readonly double _lootChance = 0.3;

        public EncounterGenerator(EnemyFactory enemyFactory, LootGenerator loot)
        {
            _enemyFactory = enemyFactory;
            _loot = loot;
        }

        public Encounter Generate()
        {
            double roll = Utils.RNG.NextDouble();

            // ───────────────────────────────────────────────
            // ENEMY
            // ───────────────────────────────────────────────
            if (roll < _enemyChance)
            {
                var enemy = _enemyFactory.CreateRandom();
                return Encounter.WithEnemy(enemy, $"Spotykasz {enemy.Name}!");
            }

            // ───────────────────────────────────────────────
            // LOOT
            // ───────────────────────────────────────────────
            if (roll < _enemyChance + _lootChance)
            {
                var item = _loot.GenerateLoot();
                return Encounter.WithLoot(item, "Znalazłeś coś.");
            }

            // ───────────────────────────────────────────────
            // EMPTY
            // ───────────────────────────────────────────────
            return Encounter.Empty();
        }
    }
}
