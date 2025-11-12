using System;

namespace TextRpg
{
    [Serializable]
    public class AdvancedWeapon : Weapon
    {
        public int CritChance { get; set; } // procentowo, np. 20
        public int CritMultiplier { get; set; } = 150; // procent, 150% obrażeń
        public int RequiredLevel { get; set; } = 1;
        public int Durability { get; set; } = -1; // -1 = nieskończona

        private static readonly Random _rng = new Random();

        public AdvancedWeapon(string name, int dmg, int critChance = 5, int reqLevel = 1, int durability = -1)
            : base(name, dmg)
        {
            CritChance = critChance;
            RequiredLevel = reqLevel;
            Durability = durability;
        }

        public int RollDamage(int basePower)
        {
            var dmg = basePower + Damage + _rng.Next(0, 3);
            if (_rng.Next(0, 100) < CritChance)
            {
                dmg = (dmg * CritMultiplier) / 100;
                Console.WriteLine("TRAFIONO KRYTYCZNIE!");
            }
            if (Durability > 0)
            {
                Durability--;
                if (Durability == 0) Console.WriteLine($"Broń {Name} straciła wytrzymałość i została zniszczona.");
            }
            return dmg;
        }
    }
}