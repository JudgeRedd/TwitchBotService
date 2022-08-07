using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace TwitchChatBot
{
    internal class sevenTv
    {
        public Dictionary<string, string> EmoteDictionary;
        public IEnumerable<string> EmoteList;
        HttpResponseMessage? response;
        string? result;
        List<Emotes>? emotes;
        HttpClient client;
        Settings? _settings;

        public sevenTv()
        {
            _settings = new configuration().settings;
            client = new HttpClient();
        }

        public async Task Setup()
        {
            response = client.GetAsync(_settings.SevenTvAPIURL).Result;
            result = response.Content.ReadAsStringAsync().Result;
            emotes = JsonConvert.DeserializeObject<List<Emotes>>(result);
        }

        public async Task Refresh(TriggerType triggerType) 
        {
            await Setup();

            while(emotes.Count() == 0)
            {
                for(int retry = 0; retry <= 3; retry++)
                {
                    await Task.Delay(5000);
                    await Setup();
                    if(emotes.Count() > 0)
                        break;

                    Console.WriteLine("No emotes found, retrying in 5 seconds");
                }
            }

            EmoteList = emotes.Select(x => x.Name);
            EmoteDictionary = emotes.ToDictionary(x => x.Name, x => x.Urls[3][1]);

            var db = new database();

            if(triggerType == TriggerType.Startup)
            {
                if(await db.CountNewEmotes(emotes) > 0)
                {
                    await db.AddNewEmotes(EmoteDictionary);
                }
            }

            if(triggerType == TriggerType.ModTriggered)
            {
                for(var retry = 0; retry < 5; retry++)
                {
                    await Task.Delay(10000);
                    await Setup();
                    if(await db.CountNewEmotes(emotes) > 0)
                        break;
                    
                    Console.WriteLine("API not updated yet, retrying in 10 seconds");
                    
                }
                
                await db.AddNewEmotes(EmoteDictionary);
            }

        }

        private struct EmoteTotals
        {
            public int id { get; set; }
            public string name { get; set; }
            public int count { get; set; }
            public string url { get; set; }
        }

        public enum TriggerType 
        {
            Startup, 
            ModTriggered
        }

    }
}
