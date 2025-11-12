using System;
using System.IO;
using System.Text.Json;

namespace TextRpg
{
    public static class SaveSystem
    {
        private static string SavePath => "save.json";

        public static void Save(Player player)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            };
            var json = JsonSerializer.Serialize(player, options);
            File.WriteAllText(SavePath, json);
            Console.WriteLine("Zapisano grę.");
        }

        public static Player? Load()
        {
            if (!File.Exists(SavePath))
            {
                Console.WriteLine("Brak zapisu gry.");
                return null;
            }

            var options = new JsonSerializerOptions
            {
                IncludeFields = true
            };
            var json = File.ReadAllText(SavePath);

            try
            {
                var player = JsonSerializer.Deserialize<Player>(json, options);
                Console.WriteLine("Wczytano grę.");
                return player;
            }
            catch
            {
                Console.WriteLine("Błąd odczytu zapisu.");
                return null;
            }
        }
    }
}