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
            Name = name; Description = desc;
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
        public HealthPotion() : base("Mikstura zdrowia", "Przywraca trochę HP") { }
        public void Consume(Player player) => player.Heal(HealAmount);
    }

    [Serializable]
    public class Weapon : Item
    {
        public int Damage { get; set; }
        public Weapon(string name, int dmg) : base(name, "Broń zwiększająca obrażenia") { Damage = dmg; }
    }
}