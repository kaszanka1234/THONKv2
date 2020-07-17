using System.ComponentModel.DataAnnotations;

namespace THONK.Database {
    public class GuildsConfig {
        [Key]
        public ulong GuildID { get; set; }
        public string Prefix { get; set; }
        public ulong GeneralChannel { get; set; }
        public ulong AnnouncementsChannel { get; set; }
        public ulong BotLogChannel { get; set; }
        public ulong LogChannel { get; set; }
    }
    
    // public class UserRecord {
    //     [Key]
    //     public ulong Id { get; set; }
    //     public int Warnings { get; set; }
    // }

    // public class GuildUsers {
    //     [Key]
    //     public ulong GuildId { get; set; }
    //     public UserRecord Users { get; set; }
    // }
}
