using System;

namespace TextRpg
{
    [Serializable]
    public class Goblin : Enemy
    {
        public bool IsPoisoning { get; private set; }
        private static readonly Random _rng = new Random();

        public Goblin() : base("Goblin", 10, 3) { }

        public override int Attack()
        {
            // Goblin ma szansę zatruć gracza - flaga, którą runner gry wykorzysta
            if (_rng.NextDouble() < 0.2)
            {
                IsPoisoning = true;
            }
            return base.Attack();
        }

        public void ApplyPoisonEffect(Player player)
        {
            if (IsPoisoning)
            {
                Console.WriteLine("Goblin zatruł gracza! Dostajesz 2 obrażeń od trucizny.");
                player.TakeDamage(2);
                // poison lasts only 2 aplikacje dla przykładu
                IsPoisoning = false;
            }
        }
    }

    [Serializable]
    public class Boss : Enemy
    {
        private static readonly Random _rng = new Random();
        public Boss(string name, int hp, int dmg) : base(name, hp, dmg) { }

        public bool TryStun()
        {
            return _rng.NextDouble() < 0.15; // 15% szansy na ogłuszenie
        }
    }
}