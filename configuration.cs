using Microsoft.Extensions.Configuration;

namespace TwitchChatBot;

internal class configuration
{
    public Settings settings;

    public configuration()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        settings = config.GetRequiredSection("Settings").Get<Settings>();
    }
}