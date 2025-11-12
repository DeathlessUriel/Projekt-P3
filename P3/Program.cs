using System;
using System.Threading.Tasks;


namespace TextRpg
{
class Program
{
static async Task Main(string[] args)
{
Console.OutputEncoding = System.Text.Encoding.UTF8;
var game = new Game();
await game.StartAsync();
}
}
}