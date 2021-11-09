using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dm.Shibalana.DiscordBot
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly Config config;
        private readonly CommandService commands;

        public HelpModule(IOptions<Config> config, CommandService commands)
        {
            this.config = config.Value;
            this.commands = commands;
        }

        [Command("help")]
        [Summary("List of commands and information for this bot")]
        [Remarks("Simply using `help` will show all commands and some summary information.\n" +
            "Aliases are also available to make sending commands faster.\n" +
            "To review command usage and remarks, use the `help <command>` with the full 'name' of the command (not an alias).\n")]
        public async Task Help(string command = "")
        {
            var output = new EmbedBuilder()
                .WithColor(Color.THEME);
            if (command == string.Empty)
            {
                var mods = commands.Modules
                    .Where(x => x.Name != GetType().Name)
                    .OrderBy(x => x.Name);
                foreach (var mod in mods)
                {
                    AddHelp(mod, ref output);
                }

                output.WithAuthor(author =>
                {
                    author.WithName($"Shibalana Price Bot v{Utils.GetVersion()}");
                }).WithFooter(footer =>
                {
                    footer.WithText($"Use '{config.BotPrefix}help <command>' to get help with a specifc command")
                        .WithIconUrl(Asset.INFO);
                });
            }
            else
            {
                var cmd = commands.Commands.FirstOrDefault(m => m.Name.ToLower() == command.ToLower());
                if (cmd == null)
                {
                    return;
                }

                output.AddField($"Command: **{cmd.Aliases.FirstOrDefault()}**",
                    $"{GetParams(cmd)}\n" +
                    $"**Summary**: {cmd.Summary}\n" +
                    $"**Remarks**: {cmd.Remarks}" +
                    $"{GetAliases(cmd)}");
            }

            await Discord.ReplyDMAsync(Context, output, deleteUserMessage: true).ConfigureAwait(false);
        }

        private void AddHelp(ModuleInfo mod, ref EmbedBuilder output)
        {
            var cmds = mod.Commands
                .OrderBy(x => x.Aliases.FirstOrDefault());
            string cmdString = string.Concat(cmds.Select(x =>
                $"— `{config.BotPrefix}{x.Aliases.FirstOrDefault()} {GetParams(x, false)}`\n" +
                $"{x.Summary}\n"));

            output.AddField($"{mod.Name} ({mod.Commands.Count})",
                cmdString.TrimEnd(", "));
            //output.AddField($"— {cmd.Aliases.FirstOrDefault()} {GetParams(cmd, false)}",
            //    $"{cmd.Summary}" +
            //    $"{GetAliases(cmd)}");
        }

        private string GetAliases(CommandInfo cmd)
        {
            string s = string.Empty;
            var aliases = cmd.Aliases.Where(x => x != cmd.Name);
            if (aliases.Any())
            {
                string aliasJoin = string.Join("|", aliases.Select(x => $"`{x}`"));
                s = $"\n**Aliases:** {aliasJoin}";
            }
            return s;
        }

        private string GetParams(CommandInfo cmd, bool withIntro = true)
        {
            string s = string.Empty;
            if (cmd.Parameters.Any())
            {
                s += (withIntro) ? "\n**Parameters**: " : string.Empty;
                foreach (var param in cmd.Parameters)
                {
                    if (param.IsOptional)
                    {
                        s += $"[{param.Name} = '{param.DefaultValue}'] ";
                    }
                    else if (param.IsMultiple)
                    {
                        s += $"*{param.Name}... ";
                    }
                    else if (param.IsRemainder)
                    {
                        s += $"...{param.Name} ";
                    }
                    else
                    {
                        s += $"<{param.Name}> ";
                    }
                }
            }
            return s.TrimEnd(" ");
        }
    }
}
