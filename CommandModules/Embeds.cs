using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace THONK.Core.CommandModules {
    [Group("embeds"),RequireUserPermission(Discord.GuildPermission.Administrator)]
    public class Embeds : ModuleBase<SocketCommandContext>{
        [Command("rules")]
        public async Task Rules(){
            EmbedBuilder[] builders = {new EmbedBuilder(),new EmbedBuilder(),new EmbedBuilder(),new EmbedBuilder()};
            foreach(var builder in builders){
                builder.WithAuthor(Context.User);
                builder.WithFooter("Last Updated");
                builder.WithCurrentTimestamp();
            }
            builders[0].WithColor(Color.Blue);
            builders[1].WithColor(Color.Red);
            builders[2].WithColor(Color.Purple);
            builders[3].WithColor(Color.Gold);
            
            builders[0].WithTitle("First and foremost");
            builders[1].WithTitle("Discord Rules");
            builders[2].WithTitle("In-game rules");
            builders[3].WithTitle("Got any suggestions?");
            
            string[] dRules = {
                "Be respectful.",
                "Sending/Linking any harmful material such as viruses, IP grabbers or harmware results in an immediate and permanent ban.",
                "Use english only in all text channels",
                "Use proper grammar and spelling and don't spam.",
                "Usage of excessive extreme innapropriate language is prohibited.",
                "Usage of excessive caps is prohibited",
                "Mentioning @everyone, the Moderators or a specific person without proper reason is prohibited.",
                "Don't post someone's personal information without permission.",
                "Don't post any material forbidden by international laws",
                "If you are offline on discord for more than 7 days you can be kicked out of clan. If you can't login for an extended period of time contact any officer or higher.",
                "Please use the same name that you use in-game. Do not use fake nickname.",
                "You can invite others to discord with this link https://discord.gg/kv74E4C",
                "Everyone who doesn't plan to join clan should get a rank of guest (/user rank guest)",
                "NO advertising of any kind on our Discord server, This includes links to sites that earn you money. Admins decide what is allowed and what is not. DONT asume anything check with an admin first.",
                "No rules apply to <#514901753863340054>, aside from 'Don't post any material forbidden by international laws'",
                " __**IT'S INADVISED TO VIEW <#514901753863340054> IF YOU ARE FAINT-HEARTED**__",
                "if you want to view nsfw channels you have to assign yourself nsfw role with /user rank nsfw-role, by doing this you confirm that you know what you are doing",
                "Most warnings will be sent by bot in private messages, having private messages disabled is not an excuse for ignoring them"
            };
            string[] gRules = {
                "All rulles that apply to discord also apply to in game chat",
                "If you are inactive for more than 30 days you can be kicked out of clan, if you can't play for extended period of time contact any officer or higher",
                "To reach the rank of soldier in clan you have to be at least MR3 and be a member of the clan for at least 5 days, if you are inactive you can be kicked out",
                "Higher ranks are given by the highest ranked members of the clan as they are needed, don't ask for a higher rank unless you have a good reason for it",
                "You can be kicked from clan if you are qualified as leech"
            };

            string r1, r2 = r1 = "";
            for(int i=0;i<dRules.Length;++i){
                r1 += $"**{i+1}.**{dRules[i]}\n";
            }
            for(int i=0;i<gRules.Length;++i){
                r2 += $"**{i+1}.**{gRules[i]}\n";
            }
            

            string[] contents = {
                "You need discord app and be on this server",
                r1,
                r2,
                "Message <@333769079569776642> or post them in <#514901783223599142>"
            };

            for(int i=0;i<builders.Length;++i){
                builders[i].WithDescription(contents[i]);
            }
            for(int i=0;i<builders.Length;++i){
                await Context.Channel.SendMessageAsync("",false,builders[i].Build());
                await Task.Delay(2800);
            }
        }

        [Command("ranks")]
        public async Task Ranks(){
            var serverRoles = Context.Guild.Roles;
            SocketRole[] roles = {
                serverRoles.Where(x=>x.Name=="Warlord").First(),
                serverRoles.Where(x=>x.Name=="General").First(),
                serverRoles.Where(x=>x.Name=="Lieutenant").First(),
                serverRoles.Where(x=>x.Name=="Sergeant").First(),
                serverRoles.Where(x=>x.Name=="Soldier").First(),
                serverRoles.Where(x=>x.Name=="Initiate").First(),
                serverRoles.Where(x=>x.Name=="Guest").First(),
                serverRoles.Where(x=>x.Name=="Visitor").First()
            };
            await Task.Run(()=>RanksTask(roles));
        }

        private async Task RanksTask(SocketRole[] roles){
            string[] contents = {
                $"{roles[0].Mention}\nLeader of the clan, person with all permissions, only warlord can ban from discord server",
                $"{roles[1].Mention}\nAdministrators of the clan they have almost all permissions, they can kick members out of the clan and construct new dojo rooms",
                $"{roles[2].Mention}\nModerators of the clan they can promote other members and mute in text channels too. If you think you should be promoted, you have trouble changing your nickname or you will be offline for a long time message one of them",
                $"{roles[3].Mention}\nPeople that help maintain the clan, they can recruit new members and queue new research, can also mute members in voice channels. If you want someone added to the clan or you see new research to be done, messeage one of them",
                $"{roles[4].Mention}\nAccepted members of the clan",
                $"{roles[5].Mention}\nEvery new member is assigned the role if initiate, if they are active and obey the rules, they are promoted to rank of soldier in a few days, else they are getting kicked",
                $"{roles[6].Mention}\nFriends of the clan that are not in the clan itself",
                $"{roles[7].Mention}\nNewcommers on the server"
            };
            
            EmbedBuilder[] builders = {new EmbedBuilder(),new EmbedBuilder(),new EmbedBuilder(),new EmbedBuilder(),new EmbedBuilder(),new EmbedBuilder(),new EmbedBuilder(),new EmbedBuilder()};
            for(int i=0;i<roles.Length;++i){
                builders[i].WithColor(roles[i].Color);
                //builders[i].WithTitle("​");
                builders[i].WithDescription(contents[i]);
            }
            

            for(int i=0;i<builders.Length;++i){
                await Context.Channel.SendMessageAsync("",false,builders[i].Build());
                await Task.Delay(2800);
            }
        }

        [Command("sguide")]
        public async Task Sguide(){
            var builder = new EmbedBuilder();
            builder.WithFooter("Last updated");
            builder.WithCurrentTimestamp();
            builder.WithAuthor(Context.User);
            builder.AddField("​",$"as a {Context.Guild.Roles.Where(x => x.Name=="Lieutenant").First().Mention} you can promote other members and moderate discord and in game chat\nYou can promote initiates to soldiers once they meet requirements");
            builder.AddField("​",$"as s {Context.Guild.Roles.Where(x => x.Name=="Sergeant").First().Mention} you can recruit new people and and **should** approve new members once they meet requirements, you can also queue new research in labs\nBefore approving someone with '/user approve @user' make sure they have correct nickname on discord and actually joined clan");
            builder.AddField("​",$"Don't recruit every random person, go through a recruitment process with everyone, preferably on voice channel. You can use questions included in <#543400258294644766>\nI trust YOUR judgement to decide if given person is going to end up as leech or not");
            builder.AddField("​",$"if you are incompetent or abusive you will be stripped of your rank");
            builder.AddField("​",$"use this link to invite new members to discord\nhttps://discord.gg/kv74E4C");
            await Context.Channel.SendMessageAsync("",false,builder.Build());
        }
        
    }
}