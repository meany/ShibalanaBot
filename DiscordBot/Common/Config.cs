using System.Collections.Generic;

namespace dm.Shibalana.DiscordBot
{
    public class Config
    {
        public string BotPrefix { get; set; }
        public string BotToken { get; set; }
        public ulong GuildId { get; set; }
        public List<ulong> ChannelIds { get; set; }
        public ulong AdminChannelId { get; set; }
        public string EmoteGood { get; set; }
        public string EmoteBad { get; set; }
        public string EmoteShiba { get; set; }
        public int RequestCooldown { get; set; }
        public List<ulong> AdminRoleIds { get; set; }
    }
}
