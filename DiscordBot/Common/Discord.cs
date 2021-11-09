using Discord;
using Discord.Commands;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dm.Shibalana.DiscordBot
{
    public static class Discord
    {
        public static EmbedBuilder OutputError(string error)
        {
            var output = new EmbedBuilder();
            output.WithColor(Color.ERROR)
                .WithAuthor(author =>
                {
                    author.WithName(error)
                        .WithIconUrl(Asset.ERROR);
                });
            return output;
        }

        public static EmbedBuilder OutputRateLimit(double seconds)
        {
            return OutputError($"You are sending bot requests too fast. Please wait another {Math.Round(seconds, 1)} seconds.");
        }

        public static EmbedBuilder OutputInfo(string title, string body)
        {
            var output = new EmbedBuilder();
            output.WithColor(Color.INFO)
                .WithAuthor(author =>
                {
                    author.WithName(title)
                        .WithIconUrl(Asset.INFO);
                })
                .WithDescription(body);
            return output;
        }

        public static EmbedBuilder OutputSuccess(string title, string body)
        {
            var output = new EmbedBuilder();
            output.WithColor(Color.SUCCESS)
                .WithAuthor(author =>
                {
                    author.WithName(title)
                        .WithIconUrl(Asset.SUCCESS);
                })
                .WithDescription(body);
            return output;
        }

        public static async Task<IUserMessage> ReplyAsync(ICommandContext ctx, EmbedBuilder embed = null, Stream file = null, string fileName = null, string message = "", bool deleteUserMessage = false, bool reply = false)
        {
            if (ctx.Channel is IDMChannel)
                return await ReplyDMAsync(ctx, embed, file, fileName, message, deleteUserMessage).ConfigureAwait(false);

            return await SendAsync(ctx, ctx.Channel.Id, embed, file, fileName, message, deleteUserMessage, reply).ConfigureAwait(false);
        }

        public static async Task<IUserMessage> ReplyDMAsync(ICommandContext ctx, EmbedBuilder embed = null, Stream file = null, string fileName = null, string message = "", bool deleteUserMessage = false)
        {
            var msg = await SendDMAsync(ctx.User, embed, file, fileName, message);

            if (deleteUserMessage && ctx.Guild != null && ctx.Message != null)
                await ctx.Message.DeleteAsync().ConfigureAwait(false);

            return msg;
        }

        public static async Task<IUserMessage> SendAsync(ICommandContext ctx, ulong channelId, EmbedBuilder embed = null, Stream file = null, string fileName = null, string message = "", bool deleteUserMessage = false, bool reply = false)
        {
            var channel = (ITextChannel)await ctx.Client.GetChannelAsync(channelId).ConfigureAwait(false);

            if (deleteUserMessage && ctx.Guild != null && ctx.Message != null)
                await ctx.Message.DeleteAsync().ConfigureAwait(false);

            MessageReference msgRef = null;
            if (reply)
                msgRef = new MessageReference(ctx.Message.Id);

            if (file != null && fileName != null)
                if (embed != null)
                    return await channel.SendFileAsync(file, fileName, embed: embed.Build(), messageReference: msgRef).ConfigureAwait(false);
                else
                    return await channel.SendFileAsync(file, fileName, messageReference: msgRef).ConfigureAwait(false);
            else if (embed != null)
                return await channel.SendMessageAsync(message, embed: embed.Build(), messageReference: msgRef).ConfigureAwait(false);
            else
                return await channel.SendMessageAsync(message, messageReference: msgRef).ConfigureAwait(false);
        }

        public static async Task<IUserMessage> SendDMAsync(IUser user, EmbedBuilder embed = null, Stream file = null, string fileName = null, string message = "")
        {
            var channel = await user.GetOrCreateDMChannelAsync().ConfigureAwait(false);

            if (file != null && fileName != null && embed != null)
                return await channel.SendFileAsync(file, fileName, embed: embed.Build()).ConfigureAwait(false);
            else if (embed != null)
                return await channel.SendMessageAsync(message, embed: embed.Build()).ConfigureAwait(false);
            else
                return await channel.SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}
