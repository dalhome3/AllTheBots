using DiscordBot.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DSharpPlus.Interactivity;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using DiscordBot.Modules.Commands;
using DiscordBot.Modules.Messaging;
using DiscordBot.Modules.Helpers;

namespace DiscordBot
{
    public class DiscordBotCore
    {
        private readonly string _config;
        private ConfigModel config;

        private DiscordClient _client;
        private CommandsNextModule _cmdModule;

        private TwitchFiltering TwitchFilter;

        private ILogger Logger;

        public DiscordBotCore(string path)
        {
            _config = path;
        }

        public async Task LaunchAsync(ILogger log)
        {
            Logger = log;

            Logger.LogInformation("Starting Discord Bot :)");

            await LoadConfig();
            SetupDiscordClient();
            CreateCommandConfig();

            _cmdModule.RegisterCommands<BaseCommands>();

            // Default help command is <prefix>help
            _cmdModule.SetHelpFormatter<BotHelpFormatter>();

            TwitchFilter = new TwitchFiltering(config);
            _client.MessageCreated += MessageRecieved;

            await _client.ConnectAsync();
            await Task.Delay(-1);
        }

        public async Task ShutdownAsync()
        {
            await _client.DisconnectAsync();
        }

        private async Task LoadConfig()
        {
            try
            {
                using(var sr = new StreamReader(_config))
                {
                    var json = await sr.ReadToEndAsync();
                    config = JsonConvert.DeserializeObject<ConfigModel>(json);
                }
                Logger.LogInformation("Discord Bot configuration successfully loaded");
            }
            catch(Exception e)
            {
                config = null;
            }
        }

        private void CreateCommandConfig()
        {
            CommandsNextConfiguration cmdConfig = new CommandsNextConfiguration()
            {
                StringPrefix = config.Prefix,
                EnableDms = true,
                EnableMentionPrefix = config.MentionBot
            };

            _cmdModule = _client.UseCommandsNext(cmdConfig);
            Logger.LogInformation("Discord Bot command module created");
        }

        private void SetupDiscordClient()
        {
            DiscordConfiguration dscConfig = new DiscordConfiguration()
            {
                Token = config.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = DSharpPlus.LogLevel.Debug,
                UseInternalLogHandler = true
            };

            _client = new DiscordClient(dscConfig);

            _client.Ready += OnClientReady;
            _client.GuildAvailable += OnGuildAvaliable;
            _client.ClientErrored += OnClientErrored;

            _client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(1)
            });
        }

        private Task OnClientErrored(ClientErrorEventArgs e)
        {
            Logger.LogError($"Error: {e.Exception.GetType()}\n\tMsg: {e.Exception.Message}\n\t StkTrc: {e.Exception.StackTrace}");
            return Task.CompletedTask;
        }

        private Task OnGuildAvaliable(GuildCreateEventArgs e)
        {
            Logger.LogInformation($"Bot now connect to server: {e.Guild.Name}");
            return Task.CompletedTask;
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            Logger.LogInformation($"Client is now ready. Prefix: [{config.Prefix}]");
            return Task.CompletedTask;
        }

        private Task MessageRecieved(MessageCreateEventArgs e)
        {
            if (!TwitchFilter.RunThroughTwitch(e.Message.Content).GetAwaiter().GetResult())
            {
                e.Channel.SendMessageAsync($"{e.Author.Mention} that message is not allowed on this server. Sorry.");

                e.Message.DeleteAsync();
            }
            return Task.CompletedTask;
        }
    }
}
