using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

namespace THONK.Configuration{
    public class Config : IConfig{
        Dictionary<ulong,ConfigMember> members;
        public IConfigMember this[ulong key]{
            get{
                if(!members.Keys.Contains(key)){
                    members[key] = new ConfigMember();
                }
                return members[key];
            }
            set{
                members[key] = (ConfigMember)value;
            }
        }
        public IConfigMember Current(){
            //ulong id;
            return new ConfigMember();
        }
        public Config(){
            members = new Dictionary<ulong, ConfigMember>();
        }
    }
    public class ConfigMember : IConfigMember{
        string _prefix;
        SocketTextChannel _general;
        SocketTextChannel _announcements;
        SocketTextChannel _botLog;
        SocketTextChannel _log;

        public ConfigMember(){
            _prefix = "/";
            _general = null;
            _announcements = null;
            _botLog = null;
            _log = null;
        }
        
        string IConfigMember.Prefix{
            get{
                return _prefix;
            }
            set{
                if(string.IsNullOrEmpty(value)){
                    throw new Exception("Prefix cannot be empty");
                }
                _prefix = value;
            }
        }
        SocketTextChannel IConfigMember.GeneralChannel{
            get{
                return _general;
            }
            set{
                _general = value;
            }
        }
        SocketTextChannel IConfigMember.AnnouncementsChannel{
            get{
                return _announcements;
            }
            set{
                _announcements = value;
            }
        }
        SocketTextChannel IConfigMember.BotLogChannel{
            get{
                return _botLog;
            }
            set{
                _botLog = value;
            }
        }
        SocketTextChannel IConfigMember.LogChannel{
            get{
                return _log;
            }
            set{
                _log = value;
            }
        }
        
    }
}