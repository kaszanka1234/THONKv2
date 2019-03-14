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
}
