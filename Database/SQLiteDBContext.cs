using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Discord.WebSocket;

namespace THONK.Database {
    public class SQLiteDBContext : DbContext {
        public DbSet<GuildsConfig> GuildConfigs { get; set; }
        public DbSet<GuildUsers> GuildUsers { get; set; }

        // Load or create* database file
        //
        // ______________________________
        // * created file will not have any tables in (look further below)
        protected override void OnConfiguring(DbContextOptionsBuilder Options) {
            Options.UseSqlite("Data Source=db.sqlite");

            // if no file is found a new one will be created
            // i haven't figured out how to generate tables on runtime so it won't work and bot will crash
            // to create database you have to:
            // 1. run the code first so it will create the correct file
            // 2. 'dotnet ef migrations add' to migrate classes into tables
            // 3. 'dotnet ef database update' to update the database with generated tables
        }

        protected override void OnModelCreating(ModelBuilder builder){
            // TODO
            //
            // generate tables on run time
        }
    }
}
