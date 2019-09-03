using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace THONK.Services{
    public class Logger{
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private string _logDirectory {get;}
        private string _logFile => Path.Combine(_logDirectory, $"{DateTime.Now.ToString("yyyy-MM-dd")}.log");
        private string _latestLogFile => Path.Combine(_logDirectory, "latest.log");

        public Logger(DiscordSocketClient client, CommandService commands){
            _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            _client = client;
            _commands = commands;
            _client.Log += LogAsyncPrivate;
            File.WriteAllBytes(_latestLogFile, new Byte[0]);
        }

        // Main logging method
        private Task LogAsyncPrivate(LogMessage msg){
            // Create new directory for logs in a file system if it doesn't exist
            if(!Directory.Exists(_logDirectory)){
                Directory.CreateDirectory(_logDirectory);
            }
            // Create a file for today if it doesn't exist and release it so other resources can use it
            /* if(!File.Exists(_logFile)){
                File.Create(_logFile).Dispose();
            }
            */
            
            // format messages in console
            /* var cc = Console.ForegroundColor;
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
            }*/
            // append date, severity, source to log message
            string logging = $"{DateTime.Now,-19} [{msg.Severity,8}] {msg.Source}: {msg.Message}";
            //Console.ForegroundColor = cc;

            // Write the message to log files
            using (StreamWriter s = new StreamWriter(_logFile,true)){
                s.WriteLineAsync(logging);
            }
            using (StreamWriter s = new StreamWriter(_latestLogFile,true)){
                s.WriteLineAsync(logging);
            }
            // return Task that logs message to console
            return Console.Out.WriteLineAsync(logging);
        }

        public async Task LogAsync(LogMessage m){
            LogMessage msg = new LogMessage(m.Severity,$"(ext){m.Source}",m.Message, m.Exception);
            await LogAsyncPrivate(msg);
        }

        public async Task LogAsync(string message, string source = "unspecified", LogSeverity severity = LogSeverity.Info, Exception exception = null){
            await LogAsyncPrivate(new LogMessage(severity,$"(ext){source}",message,exception));
        }
    }
}