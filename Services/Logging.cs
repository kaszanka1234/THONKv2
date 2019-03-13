using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace THONK.Services{
    public class Logging{
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private string _logDirectory {get;}
        private string _logFile => Path.Combine(_logDirectory, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.log");

        public Logging(DiscordSocketClient client, CommandService commands){
            _logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
            _client = client;
            _commands = commands;
            _client.Log += LogAsync;
        }
        private Task LogAsync(LogMessage msg){
            if(!Directory.Exists(_logDirectory)){
                Directory.CreateDirectory(_logDirectory);
            }
            if(!File.Exists(_logFile)){
                File.Create(_logFile).Dispose();
            }
            var cc = Console.ForegroundColor;
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            string logging = $"{DateTime.Now,-19} [{msg.Severity,8}] {msg.Source}: {msg.Message}";
            Console.ForegroundColor = cc;
            using (StreamWriter s = new StreamWriter(_logFile,true)){
                s.WriteLineAsync(logging);
            }
            return Console.Out.WriteLineAsync(logging);
        }
    }
}