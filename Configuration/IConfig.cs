
using Discord.WebSocket;
using System;

namespace THONK.Configuration{

    // Main configuration pairs guild ID with its configuration values
    // it's used like an array
    public interface IConfig{
        IConfigMember this[ulong key] {get;set;}
        IConfigMember Current();
    }

    // Set of configuration values
    // example access to prefix value of guild with ID id
    // config[id].Prefix
    //
    // pretty self explanatory
    public interface IConfigMember{
        string Prefix { get; set;}
        SocketTextChannel GeneralChannel {get;set;}
        SocketTextChannel AnnouncementsChannel {get;set;}
        SocketTextChannel BotLogChannel {get;set;}
        SocketTextChannel LogChannel {get;set;}
    }
}