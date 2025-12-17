using System;

namespace TextRpg
{
    public class PlayerCombat
    {
        private readonly Player _player;

        public Weapon? EquippedWeapon { get; private set; }

        public PlayerCombat(Player player)
        {
            _player = player;
        }

        public int Attack()
        {
            int baseAtk = _player.Stats.BaseAttack;

            if (EquippedWeapon == null)
                return baseAtk + Utils.RNG.Next(0, 3);

            if (EquippedWeapon is AdvancedWeapon aw)
            {
                if (_player.Level < aw.RequiredLevel)
                {
                    Console.WriteLine("Poziom za niski na tę broń!");
                    return baseAtk;
                }

                return aw.RollDamage(baseAtk);
            }

            return baseAtk + EquippedWeapon.Damage + Utils.RNG.Next(0, 3);
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (!_player.Inventory.Items.Contains(weapon))
            {
                Console.WriteLine("Nie masz tej broni.");
                return;
            }

            EquippedWeapon = weapon;
            Console.WriteLine($"Wyposażono {weapon.Name}");
        }
    }
}
