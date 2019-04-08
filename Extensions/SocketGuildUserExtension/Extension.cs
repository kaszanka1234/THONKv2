using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

namespace THONK.Extensions.SocketGuildUserExtension{
    public static class Extension{
        
        public static bool Authorized(this SocketGuildUser user, string minRole){
            return user.AccessLevel()<=RoleToInt(minRole);
        }

        private static int RoleToInt(string role){switch (role){
            case "Warlord":
                return 0;
            case "General":
                return 1;
            case "Lieutenant":
                return 2;
            case "Sergeant":
                return 3;
            case "Soldier":
                return 4;
            case "Guest":
                return 5;
            case "Initiate":
                return 6;
            default:
                return 7;
        }}

        public static int AccessLevel(this SocketGuildUser user){
            int accessLevel = 7;
            List<string> roleList = new List<string>();
            roleList.Insert(0,"Warlord");
            roleList.Insert(1,"General");
            roleList.Insert(2,"Lieutenant");
            roleList.Insert(3,"Sergeant");
            roleList.Insert(4,"Soldier");
            roleList.Insert(5,"Guest");
            roleList.Insert(6,"Initiate");
            foreach(var role in user.Roles){
                if(roleList.Contains(role.Name)){
                    if(accessLevel>roleList.IndexOf(role.Name)){
                        accessLevel = roleList.IndexOf(role.Name);
                    }
                }
            }
            return accessLevel;
        }

        public static bool HigherThan(this SocketGuildUser u, SocketGuildUser user){
            return u.AccessLevel()>user.AccessLevel();
        }
    }
}