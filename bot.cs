using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace TwitchChatBot
{
    internal class Bot
    {
        TwitchClient client;
        List<string> _emotes;
        public ConcurrentQueue<string> _emoteQueue = new();
        private bool isRunning;

        public Bot(List<string> emotes)
        {
            var _settings = new configuration().settings;

            string _channel = _settings.TargetChannel;
            string _oauth = _settings.OAuth;
            string _username = _settings.BotUserName;

            _emotes = emotes;

            ConnectionCredentials credentials = new ConnectionCredentials(_username, _oauth);

            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new WebSocketClient(clientOptions);

            client = new TwitchClient(customClient);
            client.Initialize(credentials, _channel);

            client.OnConnected += OnConnected;

            client.OnMessageReceived += OnMessageReceived;
            client.OnAnnouncement += OnAnnouncement;
            client.OnLog += OnLog;
            client.OnUserJoined += OnUserJoined;

            client.OnUserLeft += OnUserLeft;

            client.Connect();

        }

        private void OnUserLeft(object? sender, OnUserLeftArgs e)
        {
            Console.WriteLine($"{e.Username} left the channel");
        }

        private void OnLog(object? sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void OnAnnouncement(object? sender, OnAnnouncementArgs e)
        {
            Console.WriteLine($"{e.Announcement.Message} was announced by {e.Announcement.Login}");
        }

        private void OnJoinedChannel(object? sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine($"{e.BotUsername} has joined the channel - 2");
        }

        private void OnUserJoined(object? sender, OnUserJoinedArgs e)
        {
            Console.WriteLine($"{e.Username} has joined the channel - 1");
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            string[] words = e.ChatMessage.Message.Split(' ');
            var foundEmotes = words.Where(x => _emotes.Contains(x));
            
            if(foundEmotes.Count() > 0)
            {
                for(int i = 0; i < foundEmotes.Count(); i++)
                {
                    _emoteQueue.Enqueue(foundEmotes.ElementAt(i));
                }
                if(_emoteQueue.Count() > 0 )
                {
                    limitReached();
                }
            }

            Console.WriteLine($"{e.ChatMessage.Username} - {e.ChatMessage.Message}");
        }

        private void OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        public void limitReached()
        {
            if(!isRunning)
            {
                isRunning = true;

                var db = new database();
                Dictionary<string, int> emoteCount = db.getDictionaryEmotes();

                while(_emoteQueue.TryDequeue(out string emote))
                {
                    if(emoteCount.ContainsKey(emote))
                    {
                        emoteCount[emote]++;
                    }

                }
                Console.Error.WriteLine("Queue is empty");

                
                db.updateEmoteCount(emoteCount);
            }

            isRunning = false;
        }

    }
}