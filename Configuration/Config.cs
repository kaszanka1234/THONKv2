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
                    return new ConfigMember();
                }
                return members[key];
            }
            set{
                members[key] = (ConfigMember)value;
            }
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
        
        string IConfigMember.Prefix{
            get{
                if(_prefix==null||_prefix==""){
                    return "/";
                }else{
                    return _prefix;
                }
            }
            set{
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