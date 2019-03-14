
using Discord.WebSocket;
using System;

namespace THONK.Configuration{
    public interface IConfig{
        IConfigMember this[ulong key] {get;set;}
    }
    public interface IConfigMember{
        string Prefix { get; set;}
        SocketTextChannel GeneralChannel {get;set;}
        SocketTextChannel AnnouncementsChannel {get;set;}
        SocketTextChannel BotLogChannel {get;set;}
        SocketTextChannel LogChannel {get;set;}
    }
}