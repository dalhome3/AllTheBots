using System;
using System.IO;
using DiscordBot;
using Microsoft.Extensions.Logging;

namespace BotTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the discord bot test please use: \"exit()\" to quit");

            DiscordBotCore Bot = new DiscordBotCore(Path.GetFullPath("config.json"));

            using (var factory = LoggerFactory.Create(b => { b.AddConsole(); }))
            {
                Bot.LaunchAsync(factory.CreateLogger("Discord Bot"));

                while (Console.ReadLine() != "exit()")
                {
                }

                Bot.ShutdownAsync().GetAwaiter().GetResult();
            }
        }
    }
}
