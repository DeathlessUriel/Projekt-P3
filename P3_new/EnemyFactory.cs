using System;
using System.Collections.Generic;

namespace TextRpg
{
    public class EnemyFactory
    {
        private readonly List<(Func<Enemy> Create, double Weight)> _registry
            = new List<(Func<Enemy>, double)>();

        private double _totalWeight = 0;

        /// <summary>
        /// Rejestruje przeciwnika z wagą (np. 0.4 = 40%)
        /// </summary>
        public void Register(Func<Enemy> creator, double weight)
        {
            if (weight <= 0)
                throw new ArgumentException("Waga musi być > 0");

            _registry.Add((creator, weight));
            _totalWeight += weight;
        }

        /// <summary>
        /// Losuje przeciwnika zgodnie z wagami
        /// </summary>
        public Enemy CreateRandom()
        {
            if (_registry.Count == 0)
                throw new InvalidOperationException("Brak zarejestrowanych wrogów!");

            double roll = Utils.RNG.NextDouble() * _totalWeight;
            double current = 0;

            foreach (var entry in _registry)
            {
                current += entry.Weight;
                if (roll <= current)
                    return entry.Create();
            }

            // fallback (nie powinno się zdarzyć)
            return _registry[^1].Create();
        }
    }
}
