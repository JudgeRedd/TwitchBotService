using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace TwitchChatBot;

internal class database
{
    private string _connectionString;
    public database()
    {
        _connectionString = new configuration().settings.DefaultConnection;
    }

    public Dictionary<string, int> getDictionaryEmotes()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var emoteList = connection.Query<string>("SELECT emote_name FROM EmoteSum");
            var emoteDic = emoteList.Select((x,y) => new { x, y }).ToDictionary(z => z.x, z => 0);

            return emoteDic;
        }
    }

    public void checkExist(Dictionary<string, string> emotes)
    {
        using(var con = new SqlConnection(_connectionString))
        {
            var dbEmotes = con.Query<EmoteSum>("SELECT * FROM EmoteSum").ToList();
            var foo = emotes.Where(x => dbEmotes.All(y => y.emote_name != x.Key)).ToList();

            if(foo.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT EmoteSum (emote_name, count, emote_url) VALUES ");

                bool first = true;
                for(int i = 0; i < foo.Count(); i++)
                {
                    if(first)
                    {
                        sb.Append($"('{foo[i].Key}', 0, '{foo[i].Value}')");
                        first = false;
                    }
                    else
                    {
                        sb.Append($",('{foo[i].Key}', 0, '{foo[i].Value}')");
                    }
                }

                var InsertQuery = con.Query<EmoteSum>(sb.ToString());
            }

        }
    }

    public void updateEmoteCount(Dictionary<string, int> emoteDic)
    {
        using(var con = new SqlConnection(_connectionString))
        {
            foreach(var emote in emoteDic)
            {
                if(emote.Value > 0)
                {
                    var UpdateQuery = con.Query($"UPDATE EmoteSum SET count = count + {emote.Value} WHERE emote_name = '{emote.Key}'");
                }
            }
        }
    }

    private struct EmoteSum
    {
        public int id { get; set; }
        public string emote_name { get; set; }
        public int count { get; set; }
        public string emote_url { get; set; }
        
    }
}