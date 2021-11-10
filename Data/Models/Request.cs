using System;

namespace dm.Shibalana.Data.Models
{
    public enum RequestResponse
    {
        OK = 0,
        RateLimited = 1,
        Error = 2
    }

    public class Request
    {
        public int RequestId { get; set; }
        public RequestResponse Response { get; set; }
        public ulong DiscordUserId { get; set; }
        public string DiscordUserName { get; set; }
        public ulong DiscordChannelId { get; set; }
        public bool IsRateLimited { get; set; }
        public string Command { get; set; }
        public DateTime Date { get; set; }
    }
}
