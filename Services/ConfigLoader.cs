using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;
using THONK.Configuration;
using THONK.Database;

namespace THONK.Services{
    public class ConfigLoader{
        private IConfig _config {get;set;}
        private readonly DiscordSocketClient _client;

        public ConfigLoader(DiscordSocketClient client, IConfig config){
            _config = config;
            _client = client;
        }

        /* Load configuration from db */
        public void LoadAll(){
            /* access db */
            using(var db = new SQLiteDBContext()){
                /* load all guilds */
                foreach(var record in db.GuildConfigs){
                    LoadGuild(record);
                }
            }
        }
        /* (re)load single guild */
        public void LoadSingle(ulong guildId){
            /* access db */
            using(var db = new SQLiteDBContext()){
                /* load single record */
                LoadGuild(db.GuildConfigs.Find(guildId));
            }
        }
        /* fill config with data from record */
        void LoadGuild(GuildsConfig config){
            /* get aliases of guild and single config entry */
            var guild = _client.GetGuild(config.GuildID);
            var con = _config[guild.Id];
            /* load prefix from db record */
            con.Prefix = config.Prefix;
            /* find actuall channels from ids in db and load into config */
            con.GeneralChannel = guild.GetTextChannel(config.GeneralChannel);
            con.AnnouncementsChannel = guild.GetTextChannel(config.AnnouncementsChannel);
            con.BotLogChannel = guild.GetTextChannel(config.BotLogChannel);
            con.LogChannel = guild.GetTextChannel(config.LogChannel);
        }
        public void SaveGuildConfig(ulong guildId){
            using(var db = new SQLiteDBContext()){
                /* Get aliases to guild db entry and config entry */
                var guildRecord = db.GuildConfigs.Find(guildId);
                if(guildRecord==null){
                    guildRecord = new GuildsConfig{
                        GuildID=guildId,
                        Prefix="/",
                        GeneralChannel=0,
                        AnnouncementsChannel=0,
                        BotLogChannel=0,
                        LogChannel=0
                    };
                    db.GuildConfigs.Add(guildRecord);
                    db.SaveChanges();
                    guildRecord = db.GuildConfigs.Find(guildId);
                }
                var config = _config[guildId];
                
                /* Save guildId */
                guildRecord.GuildID = guildId;

                /* Save prefix in db */
                guildRecord.Prefix = config.Prefix;

                ///////////////////////
                SocketTextChannel channel;
                /* Save id of general channel or set it to 0 if it's not configured */
                channel = config.GeneralChannel;
                if(channel == null){
                    guildRecord.GeneralChannel = 0;
                }else{
                    guildRecord.GeneralChannel = channel.Id;
                }
                /* announcemets channel */
                channel = config.AnnouncementsChannel;
                if(channel == null){
                    guildRecord.AnnouncementsChannel = 0;
                }else{
                    guildRecord.AnnouncementsChannel = channel.Id;
                }
                /* bot log channel */
                channel = config.BotLogChannel;
                if(channel == null){
                    guildRecord.BotLogChannel = 0;
                }else{
                    guildRecord.BotLogChannel = channel.Id;
                }
                /* log channel */
                channel = config.LogChannel;
                if(channel == null){
                    guildRecord.LogChannel = 0;
                }else{
                    guildRecord.LogChannel = channel.Id;
                }
                /* Save changes in db file */
                db.GuildConfigs.Update(guildRecord);
                db.SaveChanges();
            }
        }
    }
}