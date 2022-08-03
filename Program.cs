namespace TwitchChatBot;

class Program
{
    static void Main(string[] args)
    {
        var emotes = new sevenTv();

        Bot bot = new Bot(emotes.EmoteList);
        Console.ReadLine();
    }
}