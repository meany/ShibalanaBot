using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using dm.Shibalana.Data;
using NLog;

namespace dm.Shibalana.DiscordBot
{
    public class Events
    {
        private readonly CommandService commands;
        private readonly DiscordSocketClient client;
        private readonly IServiceProvider services;
        private readonly Config config;
        private readonly AppDbContext db;
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private Timer tmrNick;

        public Events(CommandService commands, DiscordSocketClient client, IServiceProvider services, Config config, AppDbContext db)
        {
            this.commands = commands;
            this.client = client;
            this.services = services;
            this.config = config;
            this.db = db;
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage message))
                return;

            int argPos = 0;
            var context = new SocketCommandContext(client, message);

            if (message.HasStringPrefix(config.BotPrefix, ref argPos))
            {
                var result = await commands.ExecuteAsync(context, argPos, services).ConfigureAwait(false);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await context.Channel.SendMessageAsync($"{result.ErrorReason}\n" +
                        $"Try using `{config.BotPrefix}help` if you're stuck.").ConfigureAwait(false);
                return;
            }
        }

        public async Task HandleConnected()
        {
            tmrNick = new Timer(async _ =>
            {
                var item = await Common.GetAllInfo(db).ConfigureAwait(false);
                string status = $"MCap ${item.FinalPrice.CircMarketCapUSD.FormatLarge()} | !price";
                await client.SetGameAsync(status).ConfigureAwait(false);

                string nick = $"${item.FinalPrice.PriceUSD.FormatUsd(6)} " +
                    $"{item.FinalPrice.PriceUSDChange.Indicator()}";
                var guilds = client.Guilds;
                foreach (var guild in guilds)
                {
                    var me = guild.GetUser(client.CurrentUser.Id);
                    if (me.Nickname == nick)
                        return;

                    await me.ModifyAsync(x =>
                    {
                        x.Nickname = nick;
                    });
                }
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30));
        }

        public async Task HandleJoin(SocketGuildUser user)
        {
        }

        public async Task HandleLeft(SocketGuildUser user)
        {
        }

        public async Task HandleBanned(SocketUser user, SocketGuild guild)
        {
        }

        public async Task HandleReaction(Cacheable<IUserMessage, ulong> msg, ISocketMessageChannel channel, SocketReaction reaction)
        {
        }
    }
}
