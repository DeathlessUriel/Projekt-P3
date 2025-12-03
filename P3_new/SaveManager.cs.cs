using System;

namespace TextRpg
{
    public static class SaveManager
    {
        private static readonly SaveHandler _save = new SaveHandler();
        private static readonly LoadHandler _load = new LoadHandler();

        public static void Save(Player player)
        {
            _save.Save(player);
        }

        public static Player? Load()
        {
            return _load.Load();
        }
    }
}
