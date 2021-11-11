using Discord;
using Discord.Commands;
using Discord.WebSocket;
using dm.Shibalana.DiscordBot;
using dm.Shibalana.Data;
using dm.Shibalana.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace dm.Shibalana.DiscordBot.Modules
{
    [Name("1. Prices & Stats")]
    public class PriceModule : ModuleBase<SocketCommandContext>
    {
        private readonly Config config;
        private readonly AppDbContext db;
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public PriceModule(IOptions<Config> config, AppDbContext db)
        {
            this.config = config.Value;
            this.db = db;
        }

        [Command("price", RunMode = RunMode.Async)]
        [Summary("Displays the prices and statistics of Shibalana ($SHIBA).")]
        [Remarks("")]
        [Alias("p", "prices", "stat", "stats")]
        public async Task Price()
        {
            try
            {
                if (Context.Channel is IDMChannel)
                {
                    await Discord.ReplyAsync(Context,
                        message: "Please make this request in one of the official channels.")
                        .ConfigureAwait(false);
                    return;
                }

                if (!config.ChannelIds.Contains(Context.Channel.Id))
                    return;

                if (await RequestHelper.IsRateLimited(db, Context, config))
                {
                    log.Info("Request rate limited");
                    return;
                }

                using var a = Context.Channel.EnterTypingState();
                log.Info("Requesting prices and stats");

                var emotes = new Emotes(Context);
                var shiba = await emotes.Get(config.EmoteShiba).ConfigureAwait(false);

                var item = await Common.GetAllInfo(db).ConfigureAwait(false);

                string title = $"Shibalana Price and Statistics";
                string footerText = $"{item.FinalPrice.Date.ToDate()}. Powered by LanaDex & CoinGecko.";
                if (item.IsOutOfSync())
                    footerText += "\nStats might be out of sync. The admin has been contacted.";

                //string label1 = $"Full MCap: ${item.Prices[0].FullMarketCapUSD.FormatLarge()}";
                //string value1 = $"{item.FinalPrice.MarketCapUSDChange.Indicator()}{item.FinalPrice.MarketCapUSDChangePct.FormatUsd(0)}%";

                var output = new EmbedBuilder();
                output.WithColor(Color.THEME)
                .WithAuthor(author =>
                {
                    author.WithName(title);
                })
                .WithDescription($"**{item.FinalPrice.PriceUSD.FormatUsd(6)} USDC** for a single {shiba}! " +
                $"{item.FinalPrice.PriceUSDChange.Indicator()} {item.FinalPrice.PriceUSDChangePct.FormatUsd(0)}%")
                .AddField($"— Price & Trading", "```ml\n" +
                    $"100 USDC = {(item.FinalPrice.PriceSHIBAForOneUSDC * 100).FormatLarge()} SHIBA\n" +
                    //$"{label1,-20} {value1,10:C}\n" + 
                    $"Full MCap: ${item.Prices[0].FullMarketCapUSD.FormatLarge()}\n" +
                    $"Circ MCap: ${item.FinalPrice.CircMarketCapUSD.FormatLarge()}\n" +
                    $"Vol (24h): ${item.FinalPrice.VolumeUSD.FormatLarge()}\n" +
                    "```")
                .AddField($"— Stats", "```ml\n" +
                    $"Circulation: {item.Stat.Circulation.FormatLarge()} SHIBA\n" +
                    $"Holders:     {item.Stat.Holders.FormatLarge()}\n" +
                    "```")
                .WithFooter(footer =>
                {
                    footer.WithText(footerText);
                });

                await Discord.ReplyAsync(Context, output, reply: true).ConfigureAwait(false);
                log.Info("Prices and stats sent");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}