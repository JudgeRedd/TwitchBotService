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
            var emoteList = connection.Query<string>("SELECT name FROM EmoteTotals");
            var emoteDic = emoteList.Select((x,y) => new { x, y }).ToDictionary(z => z.x, z => 0);

            return emoteDic;
        }
    }

    public async Task<int> CountNewEmotes(List<Emotes> emotes)
    {
        using(var con = new SqlConnection(_connectionString))
        {
            var dbEmotes = await con.QueryAsync<EmoteTotals>("SELECT * FROM EmoteTotals");
            var toAdd = emotes.Where(x => dbEmotes.All(y => y.name != x.Name)).ToList();

            return toAdd.Count();
        }
    }

    public async Task AddNewEmotes(Dictionary<string, string> emotes)
    {
        using(var con = new SqlConnection(_connectionString))
        {
            var dbEmotes = con.Query<EmoteTotals>("SELECT * FROM EmoteTotals").ToList();
            var toAdd = emotes.Where(x => dbEmotes.All(y => y.name != x.Key)).ToList();

            if(toAdd.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT EmoteTotals (name, count, url) VALUES ");

                bool first = true;
                for(int i = 0; i < toAdd.Count(); i++)
                {
                    Console.WriteLine($"{toAdd.ElementAt(i).Key} found");
                    if(first)
                    {
                        sb.Append($"('{toAdd[i].Key}', 0, '{toAdd[i].Value}')");
                        first = false;
                    }
                    else
                    {
                        sb.Append($",('{toAdd[i].Key}', 0, '{toAdd[i].Value}')");
                    }
                }

                var InsertQuery = con.Query<EmoteTotals>(sb.ToString());
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
                    var UpdateQuery = con.Query($"UPDATE EmoteTotals SET count = count + {emote.Value} WHERE name = '{emote.Key}'");
                }
            }
        }
    }

    private struct EmoteTotals
    {
        public int id { get; set; }
        public string name { get; set; }
        public int count { get; set; }
        public string url { get; set; }
        
    }
}