using System;

namespace TextRpg
{
    [Serializable]
    public class LimitedUseItem : Item, IConsumable
    {
        public int UsesLeft { get; set; }
        public Action<Player>? Effect { get; set; }

        public LimitedUseItem(string name, int uses, Action<Player> effect, string desc = "") : base(name, desc)
        {
            UsesLeft = uses;
            Effect = effect;
        }

        public void Consume(Player player)
        {
            if (UsesLeft <= 0) {
                Console.WriteLine("Przedmiot nie ma już użyć.");
                return;
            }
            Effect?.Invoke(player);
            UsesLeft--;
            Console.WriteLine($"Pozostało użyć: {UsesLeft}");
        }
    }
}