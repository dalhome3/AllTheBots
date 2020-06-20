using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DiscordBot.Modules.Commands
{
    class BaseCommands
    {
        [Command("ping")]
        [Description("Ping command gives bot ping infomation")]
        [Aliases("pong")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
        }

        [Command("hello")]
        [Description("Say hello to the discord bot")]
        [Aliases("hi", "yo", "waddup")]
        public async Task Hello(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var mention = ctx.Member.Mention;

            await ctx.RespondAsync($"Hi {mention}, how are you doing?");
        }
    }
}
