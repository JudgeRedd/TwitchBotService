namespace TwitchChatBot
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Emotes
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("owner")]
        public Owner Owner { get; set; }

        [JsonProperty("visibility")]
        public long Visibility { get; set; }

        [JsonProperty("visibility_simple")]
        public object[] VisibilitySimple { get; set; }

        [JsonProperty("mime")]
        public string Mime { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("tags")]
        public object Tags { get; set; }

        [JsonProperty("width")]
        public long[] Width { get; set; }

        [JsonProperty("height")]
        public long[] Height { get; set; }

        [JsonProperty("urls")]
        public string[][] Urls { get; set; }
    }

    public partial class Owner
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("twitch_id")]
        public long TwitchId { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("role")]
        public Role Role { get; set; }
    }

    public partial class Role
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("position")]
        public long Position { get; set; }

        [JsonProperty("color")]
        public long Color { get; set; }

        [JsonProperty("allowed")]
        public long Allowed { get; set; }

        [JsonProperty("denied")]
        public long Denied { get; set; }
    }

    public partial class Url
    {
        public long? Integer { get; set; }
        public string PurpleUri { get; set; }
    }
}
