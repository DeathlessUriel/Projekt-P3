using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRpg
{
    [Serializable]
    public class Inventory
    {
        public int Capacity { get; private set; }
        private readonly List<Item> _items = new List<Item>();

        public IReadOnlyList<Item> Items => _items.AsReadOnly();

        public Inventory(int capacity = 10)
        {
            Capacity = capacity;
        }

        public bool Add(Item item)
        {
            if (_items.Count >= Capacity)
            {
                Console.WriteLine("Ekwipunek pełny!");
                return false;
            }
            _items.Add(item);
            return true;
        }

        public bool Remove(Item item) => _items.Remove(item);

        public Item? GetAt(int index) => (index >= 0 && index < _items.Count) ? _items[index] : null;

        public int Count => _items.Count;
    }
}