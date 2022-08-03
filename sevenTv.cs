using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace TwitchChatBot
{
    internal class sevenTv
    {
        public Dictionary<string, string> EmoteDictionary;
        public List<string> EmoteList;
        
        public sevenTv()
        {
            getEmotes();
        }

        private void getEmotes()
        {
            var _settings = new configuration().settings;

            HttpClient client = new HttpClient();

            var response = client.GetAsync(_settings.SevenTvAPIURL).Result;

            var result = response.Content.ReadAsStringAsync().Result;

            var json = JsonConvert.DeserializeObject<List<Emotes>>(result);

            EmoteList = json.Select(x => x.Name).ToList();
            EmoteDictionary = json.ToDictionary(x => x.Name, x => x.Urls[3][1]);

            var db = new database();
            db.checkExist(EmoteDictionary);

        }

        private struct EmoteSum
        {
            public int id { get; set; }
            public string emote_name { get; set; }
            public int count { get; set; }
            public string emote_url { get; set; }
            
        }

    }
}
