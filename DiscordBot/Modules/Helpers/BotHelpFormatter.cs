using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;

namespace DiscordBot.Modules.Helpers
{
    class BotHelpFormatter : IHelpFormatter
    {
        private StringBuilder HelpString { get;  }
        public BotHelpFormatter()
        {
            HelpString = new StringBuilder();
        }

        public CommandHelpMessage Build()
        {
            return new CommandHelpMessage(this.HelpString.ToString().Replace("\r\n", "\n"));
        }

        public IHelpFormatter WithAliases(IEnumerable<string> aliases)
        {
            this.HelpString.Append("Aliases: ")
                .AppendLine(string.Join(", ", aliases))
                .AppendLine();

            return this;
        }

        public IHelpFormatter WithArguments(IEnumerable<CommandArgument> arguments)
        {
            this.HelpString.Append("Arguments: ")
                .AppendLine(string.Join(", ", arguments.Select(xarg => $"{xarg.Name} ({xarg.Type.ToUserFriendlyName()})")))
                .AppendLine();

            return this;
        }

        public IHelpFormatter WithCommandName(string name)
        {
            this.HelpString.Append("Command: ")
                .AppendLine(Formatter.Bold(name))
                .AppendLine();

            return this;
        }

        public IHelpFormatter WithDescription(string description)
        {
            this.HelpString.Append("Description: ")
                .AppendLine(description)
                .AppendLine();

            return this;
        }

        public IHelpFormatter WithGroupExecutable()
        {
            this.HelpString.AppendLine("This group is a standalone command.")
                .AppendLine();

            return this;
        }

        public IHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            this.HelpString.AppendLine("Subcommands: ");

            foreach (var c in subcommands)
            {
                this.WithCommandName(c.Name);
                this.WithDescription(c.Description);
                if(c.Aliases != null)
                    this.WithAliases(c.Aliases);
            }

            this.HelpString.AppendLine();

            return this;
        }
    }
}
