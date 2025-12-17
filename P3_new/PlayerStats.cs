using System;

namespace TextRpg
{
    public class PlayerStats
    {
        public int HP { get; private set; }
        public int MaxHP { get; private set; }
        public int BaseAttack { get; private set; }

        public int Level { get; private set; }
        public int XP { get; private set; }

        public PlayerStats()
        {
            Level = 1;
            XP = 0;
            MaxHP = 30;
            HP = MaxHP;
            BaseAttack = 3;
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

        public void GainXP(int amount)
        {
            XP += amount;

            while (XP >= XPToLevelUp())
            {
                XP -= XPToLevelUp();
                LevelUp();
            }
        }

        private int XPToLevelUp() => 10 + Level * 5;

        private void LevelUp()
        {
            Level++;
            MaxHP += 5;
            HP = MaxHP;
            BaseAttack += 1;

            Console.WriteLine($"Awansujesz na poziom {Level}!");
        }
    }
}
