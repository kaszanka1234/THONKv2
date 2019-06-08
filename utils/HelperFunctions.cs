using System;
using Discord.WebSocket;

namespace THONK.utils{
    public static class HelperFunctions{
        public static string UserIdentity(SocketGuildUser user){
            string ret = $"{user.Mention} **{(string.IsNullOrEmpty(user.Nickname)?user.Username:user.Nickname)}** ({user.Id})";
            return ret;
        }

        public static string NicknameOrUsername(SocketGuildUser user){
            string ret = string.IsNullOrEmpty(user.Nickname)?user.Username:user.Nickname;
            return ret;
        }
    }
}