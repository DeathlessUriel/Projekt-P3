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
            Name = name; HP = hp; Damage = dmg;
        }

        // Ustawiamy virtual, aby klasy pochodne mogły override'ować atak
        public virtual int Attack() => Damage + new Random().Next(0, 3);

        public void TakeDamage(int d)
        {
            HP -= d;
            if (HP < 0) HP = 0;
        }
    }
}
