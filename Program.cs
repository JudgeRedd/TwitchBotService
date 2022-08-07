namespace TwitchChatBot;

class Program
{
    static async Task Main(string[] args)
    {
        Bot bot = new Bot();
        await bot.Run();

        await Task.Delay(-1);
    }
}
