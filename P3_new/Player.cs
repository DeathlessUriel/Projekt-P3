using System;

namespace TextRpg
{
    [Serializable]
    public class Player
    {
        public string Name { get; }

        public PlayerStats Stats { get; }
        public PlayerCombat Combat { get; }
        public PlayerInventory InventoryManager { get; }

        public int Gold { get; set; }

        public bool IsAlive => Stats.HP > 0;

        public Player(string name)
        {
            Name = name;

            Stats = new PlayerStats();
            Combat = new PlayerCombat(this);
            InventoryManager = new PlayerInventory(this);
        }

        // ───── delegaty (kompatybilność wsteczna) ─────

        public int HP => Stats.HP;
        public int MaxHP => Stats.MaxHP;
        public int Level => Stats.Level;
        public int XP => Stats.XP;
        
        public int BaseAttack => Stats.BaseAttack;

        public Weapon? EquippedWeapon => Combat.EquippedWeapon;
        public Inventory Inventory => InventoryManager.Inventory;

        public int Attack() => Combat.Attack();
        public void TakeDamage(int dmg) => Stats.TakeDamage(dmg);
        public void Heal(int amount) => Stats.Heal(amount);

        public void GainXP(int xp) => Stats.GainXP(xp);
        public void EquipWeapon(Weapon weapon) => Combat.EquipWeapon(weapon);

        public void AddItem(Item item) => InventoryManager.AddItem(item);
        public void RemoveItem(Item item) => InventoryManager.RemoveItem(item);
    }
}
