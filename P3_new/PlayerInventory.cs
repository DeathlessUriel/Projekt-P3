using System;

namespace TextRpg
{
    public class PlayerInventory
    {
        private readonly Player _player;

        public Inventory Inventory { get; } = new Inventory(12);

        public PlayerInventory(Player player)
        {
            _player = player;
        }

        public void AddItem(Item item)
        {
            if (!Inventory.Add(item))
                return;

            Console.WriteLine($"Dodano: {item.Name}");
        }

        public void RemoveItem(Item item)
        {
            Inventory.Remove(item);
        }

        public void UseItem(Item item)
        {
            if (item is HealthPotion hp)
            {
                hp.Consume(_player);
                Inventory.Remove(item);
            }
        }
    }
}
