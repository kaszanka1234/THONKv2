using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Discord.WebSocket;

namespace THONK.Database {
    public class SQLiteDBContextStore : DbContext {
        public DbSet<GuildsConfig> GuildConfigs { get; set; }
        public DbSet<GuildUsers> GuildUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder Options) {
            Options.UseSqlite("Data Source=db.sqlite");
        }
    }
    public class GuildData {
        public string Prefix { get; set; }
        public SocketTextChannel GeneralChannel { get; set; }
        public SocketTextChannel AnnouncementsChannel { get; set; }
        public SocketTextChannel BotLogChannel { get; set; }
        public SocketTextChannel LogChannel { get; set; }
        public Dictionary<long,GuildUsers> Users { get; set; }
    }
}
