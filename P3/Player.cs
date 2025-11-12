using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRpg
{
    [Serializable]
    public class Player
    {
        public string Name { get; set; }
        public int HP { get; private set; }
        public int MaxHP { get; private set; }
        public int BaseAttack { get; private set; }
        public Inventory Inventory { get; private set; } = new Inventory(12);
        public int Gold { get; set; } = 0;

        public int Level { get; private set; } = 1;
        public int XP { get; private set; } = 0;
        public AdvancedWeapon? EquippedWeapon { get; private set; }

        public bool IsAlive => HP > 0;

        public Player(string name)
        {
            Name = name;
            MaxHP = 30;
            HP = MaxHP;
            BaseAttack = 3;
        }

        public int Attack()
        {
            if (EquippedWeapon != null)
            {
                // sprawdź wymagania
                if (Level < EquippedWeapon.RequiredLevel)
                {
                    Console.WriteLine($"Poziom za niski, aby użyć {EquippedWeapon.Name}. Używasz broni podstawowej.");
                    return BaseAttack + new Random().Next(0, 3);
                }

                var dmg = EquippedWeapon.RollDamage(BaseAttack);
                if (EquippedWeapon.Durability == 0)
                {
                    // jeśli zniszczona, odejmij z ekwipunku
                    Inventory.Remove(EquippedWeapon);
                    EquippedWeapon = null;
                }
                return dmg;
            }
            return BaseAttack + new Random().Next(0, 3);
        }

        public void Equip(AdvancedWeapon w)
        {
            if (!Inventory.Items.Contains(w))
            {
                Console.WriteLine("Nie masz tej broni w ekwipunku.");
                return;
            }
            EquippedWeapon = w;
            Console.WriteLine($"Wyposażono {w.Name}.");
        }

        public void TakeDamage(int dmg)
        {
            HP -= dmg;
            if (HP < 0) HP = 0;
        }

        public void Heal(int amount)
        {
            HP += amount;
            if (HP > MaxHP) HP = MaxHP;
        }

        public void AddItem(Item item)
        {
            if (Inventory.Add(item))
            {
                if (item is AdvancedWeapon aw) Console.WriteLine($"Dodano broń {aw.Name} (+{aw.Damage})");
                else Console.WriteLine($"Dodano przedmiot: {item.Name}");
            }
        }

        public void GainXP(int amount)
        {
            XP += amount;
            Console.WriteLine($"Zdobywasz {amount} XP.");
            var needed = XPToLevelUp();
            while (XP >= needed)
            {
                XP -= needed;
                LevelUp();
                needed = XPToLevelUp();
            }
        }

        private int XPToLevelUp() => 10 + Level * 5;

        private void LevelUp()
        {
            Level++;
            MaxHP += 5;
            HP = MaxHP;
            BaseAttack += 1;
            Console.WriteLine($"Awansujesz na poziom {Level}! HP i siła wzrastają.");
        }
    }
}