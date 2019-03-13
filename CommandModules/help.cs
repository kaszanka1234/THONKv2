using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace THONK.Core.CommandModules {
    [Group("help")]
    class Help : ModuleBase<SocketCommandContext>{
        private readonly CommandService _service;
        private readonly IConfigurationRoot _config;

        public Help(CommandService service, IConfigurationRoot config)
        {
            _service = service;
            _config = config;
        }
        [Command("")]
        public async Task Default(){
            try {
            string prefix = _config["prefix"];
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "These are the commands you can use"
            };
            
            foreach (var module in _service.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        description += $"{prefix}{cmd.Aliases.First()}\n";
                }
                
                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }catch(Exception e){Console.WriteLine(e.ToString());}}
    }
}