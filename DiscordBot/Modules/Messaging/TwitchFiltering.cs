using DiscordBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Core.Exceptions;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Helix.Models.Moderation.CheckAutoModStatus;
using TwitchLib.Api.Helix.Models.Users;

namespace DiscordBot.Modules.Messaging
{
    class TwitchFiltering
    {
        private readonly ConfigModel _config;
        private TwitchAPI API;
        private Random _rand;

        private User BottingUser;

        public TwitchFiltering(ConfigModel config)
        {
            _config = config;

            _rand = new Random();

            CreateAPI();
        }

        private Task CreateAPI()
        {
            API = new TwitchAPI();

            API.Settings.ClientId = _config.Twitch_ClientID;
            API.Settings.Secret = _config.Twitch_ClientSecret;

            API.Settings.Scopes = new List<AuthScopes>();
            API.Settings.Scopes.Add(AuthScopes.Any);
            API.Settings.Scopes.Add(AuthScopes.Helix_User_Read_Email);
            API.Settings.Scopes.Add(AuthScopes.Helix_Moderation_Read);
            API.Settings.Scopes.Add(AuthScopes.Channel_Read);

            //var token = API.Helix.Entitlements.GetAccessToken();

            var usrs = API.Helix.Users.GetUsersAsync(logins: new List<string>() { "dalvsbotting" }).GetAwaiter().GetResult();
            BottingUser = usrs.Users[0];

            return Task.CompletedTask;

        }

        public async Task<bool> RunThroughTwitch(string message)
        {
            return true;

            var msg = new Message()
            {
                MsgId = "" + _rand.Next(10000),
                MsgText = true, // this should be "message" but there is a bug with the API
                UserId = BottingUser.Id
            };
            var responce = await API.Helix.Moderation.CheckAutoModStatusAsync(new List<Message>() { msg }, _config.Twitch_AutoModBroadcaster);
            return responce.Data[0].IsPermitted;
        }
    }
}
