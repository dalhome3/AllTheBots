using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models
{
    class ConfigModel
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
        public bool MentionBot { get; set; }

        public string Twitch_ClientID { get; set; }
        public string Twitch_ClientSecret { get; set; }
        public string Twitch_AutoModBroadcaster { get; set; }
    }
}
