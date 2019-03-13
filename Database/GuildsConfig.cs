using System.ComponentModel.DataAnnotations;

namespace THONK.Database {
    public class GuildsConfig {
        [Key]
        public ulong GuildID { get; set; }
        public string Prefix { get; set; }
        public ulong ChannelGeneral { get; set; }
        public ulong ChannelAnnouncements { get; set; }
        public ulong ChannelBotLog { get; set; }
        public ulong ChannelLog { get; set; }
    }
    public class UserRecord {
        [Key]
        public ulong Id { get; set; }
        public int Warnings { get; set; }
    }
    public class GuildUsers {
        [Key]
        public ulong GuildId { get; set; }
        public UserRecord Users { get; set; }
    }
}
