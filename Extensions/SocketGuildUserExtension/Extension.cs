using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

namespace THONK.Extensions.SocketGuildUserExtension{
    public static class Extension{
        
        public static bool Authorized(this SocketGuildUser user, string minRole){
            return user.AccessLevel()>=RoleToInt(minRole);
        }

        private static int RoleToInt(string role){switch (role){
            case "Warlord":
                return 10;
            case "General":
                return 9;
            case "Lieutenant":
                return 8;
            case "Sergeant":
                return 6;
            case "Soldier":
                return 4;
            case "Guest":
                return 3;
            case "Initiate":
                return 2;
            default:
                return 0;
        }}

        public static int AccessLevel(this SocketGuildUser user){
            //if(user.Guild.Owner == user)return 11;
            int accessLevel = 0;
            foreach(var role in user.Roles){
                int tmp = RoleToInt(role.Name);
                if(tmp>accessLevel){
                    accessLevel = tmp;
                }
            }
            return accessLevel;
        }

        public static bool HigherThan(this SocketGuildUser u, SocketGuildUser user){
            return u.AccessLevel()>user.AccessLevel();
        }
    }
}