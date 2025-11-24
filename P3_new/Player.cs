using System;

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

        public Weapon? EquippedWeapon { get; private set; }

        public bool IsAlive => HP > 0;

        public Player(string name)
        {
            Name = name;

            MaxHP = 30;
            HP = MaxHP;
            BaseAttack = 3;
        }

        // ────────────────────────────────────────────────
        // Atak gracza — broń zwykła / zaawansowana lub brak broni
        // ────────────────────────────────────────────────
        public int Attack()
        {
            if (EquippedWeapon != null)
            {
                // advanced weapon
                if (EquippedWeapon is AdvancedWeapon aw)
                {
                    if (Level < aw.RequiredLevel)
                    {
                        Console.WriteLine($"Poziom za niski, aby użyć {aw.Name}. Używasz ataku podstawowego.");
                        return BaseAttack + Utils.RNG.Next(0, 3);
                    }

                    return aw.RollDamage(BaseAttack);
                }
                else
                {
                    // zwykła broń
                    return BaseAttack + EquippedWeapon.Damage + Utils.RNG.Next(0, 3);
                }
            }

            // bez broni
            return BaseAttack + Utils.RNG.Next(0, 3);
        }

        // ────────────────────────────────────────────────
        // Wyposażanie broni
        // ────────────────────────────────────────────────
        public void EquipWeapon(Weapon weapon)
        {
            if (!Inventory.Items.Contains(weapon))
            {
                Console.WriteLine("Nie masz tej broni w ekwipunku.");
                return;
            }

            if (weapon is AdvancedWeapon aw)
            {
                if (Level < aw.RequiredLevel)
                {
                    Console.WriteLine(
                        $"Poziom {Level} jest za niski, wymagana jest {aw.RequiredLevel} aby używać {aw.Name}."
                    );
                    return;
                }
            }

            EquippedWeapon = weapon;
            Console.WriteLine($"Wyposażono: {weapon.Name}");
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

        // ────────────────────────────────────────────────
        // Dodawanie przedmiotu
        // ────────────────────────────────────────────────
        public void AddItem(Item item)
        {
            if (Inventory.Add(item))
            {
                if (item is AdvancedWeapon aw)
                    Console.WriteLine($"Dodano broń {aw.Name} (+{aw.Damage})");
                else if (item is Weapon w)
                    Console.WriteLine($"Dodano broń {w.Name} (+{w.Damage})");
                else
                    Console.WriteLine($"Dodano przedmiot: {item.Name}");
            }
        }

        // ────────────────────────────────────────────────
        // System doświadczenia / levelowania
        // ────────────────────────────────────────────────
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

        private int XPToLevelUp()
            => 10 + Level * 5;

        private void LevelUp()
        {
            Level++;
            MaxHP += 5;
            HP = MaxHP;
            BaseAttack += 1;

            Console.WriteLine($"Awansujesz na poziom {Level}! HP i siła wzrastają.");
        }

        // pomocnicze — usuwanie przedmiotu (np. mikstura po użyciu)
        public void RemoveItem(Item item)
        {
            Inventory.Remove(item);
        }
    }
}
