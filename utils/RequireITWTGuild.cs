using System;
using System.Threading.Tasks;
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;

namespace THONK.utils{
    // Inherit from PreconditionAttribute
    public class RequireITWTGuild : PreconditionAttribute{
        private readonly ulong[] allowedGuilds;

        public RequireITWTGuild() => allowedGuilds = new ulong[]{
            488843604731887641,
            514897876774551552
        };

        // Override the CheckPermissions method
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services){
            // Check if this user is a Guild User
            if (context.User is SocketGuildUser gUser){
                // If this command was executed in correct guild, return a success
                if (allowedGuilds.Contains(context.Guild.Id))
                    // Since no async work is done, the result has to be wrapped with `Task.FromResult` to avoid compiler errors
                    return Task.FromResult(PreconditionResult.FromSuccess());
                    // Since it wasn't, fail
                else
                    return Task.FromResult(PreconditionResult.FromError("This cannot be used here"));
            }
            else
                return Task.FromResult(PreconditionResult.FromError("This cannot be used here"));
        }
    }
}