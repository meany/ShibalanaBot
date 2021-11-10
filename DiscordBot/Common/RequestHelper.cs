using Discord;
using Discord.Commands;
using dm.Shibalana.Data;
using dm.Shibalana.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dm.Shibalana.DiscordBot
{
    public class RequestResult
    {
        public Request Request { get; set; }
        public bool AlreadyWarned { get; set; }
        public double RequestCooldownSeconds { get; set; }
    }

    public static class RequestHelper
    {
        public static async Task<bool> IsRateLimited(AppDbContext db, ICommandContext context, Config config)
        {
            var res = await CreateAndCheckRateLimit(db, context, config.RequestCooldown);
            if (res.Request.IsRateLimited)
            {
                if (res.AlreadyWarned)
                    return true;

                await context.Message.AddReactionAsync(new Emoji(config.EmoteBad)).ConfigureAwait(false);
                await Discord.ReplyDMAsync(context,
                   Discord.OutputRateLimit(res.RequestCooldownSeconds)).ConfigureAwait(false);

                res.Request.Response = RequestResponse.RateLimited;
                await Save(db, res).ConfigureAwait(false);

                return true;
            }

            await Save(db, res).ConfigureAwait(false);
            return false;
        }

        private static async Task<RequestResult> CreateAndCheckRateLimit(AppDbContext db,
            ICommandContext context, int seconds)
        {
            // TODO: command granularity

            var user = context.User;
            string message = context.Message.ToString();
            ulong channelId = context.Channel.Id;
            bool rateLimited = false;
            bool alreadyWarned = false;
            var now = DateTime.Now;
            var span = new TimeSpan();
            var lastReq = await db.Requests
                .AsNoTracking()
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync(x => //x.DiscordUserId == user.Id &&
                    //x.Command == message &&
                    x.DiscordChannelId == channelId &&
                    x.IsRateLimited == false)
                .ConfigureAwait(false);

            if (lastReq != null)
            {
                span = lastReq.Date.AddSeconds(seconds) - now;
                rateLimited = span.TotalSeconds > 0;

                var warnReq = await db.Requests
                    .AsNoTracking()
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefaultAsync(x => //x.DiscordUserId == user.Id &&
                        //x.Command == message &&
                        x.DiscordChannelId == channelId &&
                        x.IsRateLimited == true)
                    .ConfigureAwait(false);

                if (warnReq != null)
                    alreadyWarned = warnReq.Date > lastReq.Date;
            }

            return new RequestResult
            {
                Request = new Request
                {
                    Command = context.Message.ToString(),
                    Date = now,
                    DiscordUserId = user.Id,
                    DiscordUserName = user.Username,
                    DiscordChannelId = channelId,
                    IsRateLimited = rateLimited
                },
                AlreadyWarned = alreadyWarned,
                RequestCooldownSeconds = span.TotalSeconds
            };
        }

        public static async Task Save(AppDbContext db, RequestResult res)
        {
            db.Requests.Add(res.Request);
            await db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
