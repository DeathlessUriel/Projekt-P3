using System;

namespace TextRpg
{
    [Serializable]
    public class Enemy
    {
        public string Name { get; set; }
        public int HP { get; private set; }
        public int Damage { get; set; }

        public bool IsAlive => HP > 0;

        public Enemy(string name, int hp, int dmg)
        {
            Name = name;
            HP = hp;
            Damage = dmg;
        }

        public virtual int Attack()
        {
            return Damage + Utils.RNG.Next(0, 3);
        }

        public void TakeDamage(int dmg)
        {
            HP -= dmg;
            if (HP < 0) HP = 0;
        }
    }

    // ────────────────────────────────────────────────
    // Goblin (z trucizną)
    // ────────────────────────────────────────────────
    [Serializable]
    public class Goblin : Enemy
    {
        public bool IsPoisoning { get; private set; }

        public Goblin() : base("Goblin", 10, 3) { }

        public override int Attack()
        {
            if (Utils.RNG.NextDouble() < 0.20)
                IsPoisoning = true;

            return base.Attack();
        }

        public void ApplyPoisonEffect(Player player)
        {
            if (IsPoisoning)
            {
                Console.WriteLine("Goblin zatruł gracza! Otrzymujesz 2 obrażeń od trucizny!");
                player.TakeDamage(2);
                IsPoisoning = false;
            }
        }
    }

    // ────────────────────────────────────────────────
    // Wilk – szybki, nieregularne obrażenia
    // ────────────────────────────────────────────────
    [Serializable]
    public class Wolf : Enemy
    {
        public Wolf() : base("Wilk", 12, 4) { }

        public override int Attack()
        {
            return Damage + Utils.RNG.Next(0, 4);
        }
    }

    // ────────────────────────────────────────────────
    // Bandyta – czasami mocny cios
    // ────────────────────────────────────────────────
    [Serializable]
    public class Bandit : Enemy
    {
        public Bandit() : base("Bandyta", 14, 5) { }

        public override int Attack()
        {
            if (Utils.RNG.NextDouble() < 0.15)
                return Damage + 6;

            return base.Attack();
        }
    }

    // ────────────────────────────────────────────────
    // Ork – duży HP i stabilne obrażenia
    // ────────────────────────────────────────────────
    [Serializable]
    public class Orc : Enemy
    {
        public Orc() : base("Ork", 20, 6) { }

        public override int Attack()
        {
            return Damage + Utils.RNG.Next(0, 2);
        }
    }

    // ────────────────────────────────────────────────
    // Boss – opcjonalny, ma szansę ogłuszyć
    // ────────────────────────────────────────────────
    [Serializable]
    public class Boss : Enemy
    {
        public Boss(string name, int hp, int dmg) : base(name, hp, dmg) { }

        public bool TryStun()
        {
            return Utils.RNG.NextDouble() < 0.15;
        }
    }
}
