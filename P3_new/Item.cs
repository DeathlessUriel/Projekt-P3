using System;

namespace TextRpg
{
    [Serializable]
    public abstract class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }

        protected Item(string name, string desc = "")
        {
            Name = name; 
            Description = desc;
        }
    }

    public interface IConsumable
    {
        void Consume(Player player);
    }

    [Serializable]
    public class HealthPotion : Item, IConsumable
    {
        public int HealAmount { get; set; } = 15;

        public HealthPotion() 
            : base("Mikstura zdrowia", "Przywraca trochę HP") 
        { }

        public void Consume(Player player) 
            => player.Heal(HealAmount);
    }

    [Serializable]
    public class Weapon : Item
    {
        public int Damage { get; set; }

        public Weapon(string name, int dmg) 
            : base(name, "Broń zwiększająca obrażenia")
        {
            Damage = dmg;
        }
    }

    [Serializable]
    public class AdvancedWeapon : Weapon
    {
        public int CritChance { get; set; } = 5;
        public int CritMultiplier { get; set; } = 150; // 150% obrażeń
        public int RequiredLevel { get; set; } = 1;

        public AdvancedWeapon(string name, int dmg, int critChance = 5, int reqLevel = 1)
            : base(name, dmg)
        {
            CritChance = critChance;
            RequiredLevel = reqLevel;
        }

        public int RollDamage(int basePower)
        {
            var dmg = basePower + Damage + Utils.RNG.Next(0, 3);

            if (Utils.RNG.Next(0, 100) < CritChance)
            {
                dmg = (dmg * CritMultiplier) / 100;
                Console.WriteLine("TRAFIENIE KRYTYCZNE!");
            }

            return dmg;
        }
    }

    [Serializable]
    public class ItemGold : Item
    {
        public int Amount { get; set; }

        public ItemGold(int amount) 
            : base($"Złoto x{amount}", "Monety")
        {
            Amount = amount;
        }
    }
}
