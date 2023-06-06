using Discord;
using Discord.Audio;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PowerBot.Data.Models;
using PowerBot.DataModel;
using PowerBot.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PowerBot.Module
{
    public class Slash
    {
        private readonly DiscordSocketClient _discord;
        private readonly DataService _dataService;
        private readonly CommonService _commonService;
        public Slash(IServiceProvider services, DataService dataService, CommonService commonService)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _discord.SlashCommandExecuted += SlashHandler;
            _dataService = dataService;
            _commonService = commonService;
        }

        #region SlashBuilder
        public async Task buildSlash()
        {
            var guild = _discord.GetGuild(1086008152056672277);
            await guild.DownloadUsersAsync();
            var ping = new SlashCommandBuilder()
                .WithName("pingpower")
                .WithDescription("ping");

            var createRole = new SlashCommandBuilder()
                .WithName("powerrole")
                .WithDescription("power vip role creater")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("rolename")
                    .WithDescription("role name")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.String)
                    )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("for")
                    .WithDescription("who is the user for")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.User)
                    )
                .AddOption(new SlashCommandOptionBuilder()
                        .WithName("color")
                        .WithDescription("role color")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.String)
                        );
            var giveRole = new SlashCommandBuilder()
                .WithName("givepowerrole")
                .WithDescription("give user your role")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("user")
                    .WithDescription("user to add to your role")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.User)
                    );
            var removeRole = new SlashCommandBuilder()
                .WithName("removepowerrole")
                .WithDescription("remove user from your role")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("user")
                    .WithDescription("user to remove to your role")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.User)
                    );
            var powerLink = new SlashCommandBuilder()
                .WithName("powerlink")
                .WithDescription("link users to role")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("user")
                    .WithDescription("user to give role")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.User)
                    )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("role")
                    .WithDescription("role to give")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Role)
                    );
            var powerMc = new SlashCommandBuilder()
                .WithName("powermc")
                .WithDescription("Verify your MC Account")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("minecraftusername")
                    .WithDescription("Your Mc Username")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.String)
                    );
            var powerStats = new SlashCommandBuilder()
               .WithName("powerstats")
               .WithDescription("Server Stats");
            var powerStatsId = new SlashCommandBuilder()
               .WithName("powerstatsid")
               .WithDescription("Server Stats")
               .WithDefaultMemberPermissions(permissions: GuildPermission.ManageGuild)
               .AddOption(new SlashCommandOptionBuilder()
                    .WithName("user")
                    .WithDescription("user")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.User)
                        );
            var powerBanner = new SlashCommandBuilder()
                .WithName("powerbanner")
                .WithDescription("Get user banner")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("user")
                    .WithDescription("Discord user")
                    .WithType(ApplicationCommandOptionType.User)
                    );
            var sendmsg = new SlashCommandBuilder()
               .WithName("pmsg")
               .WithDescription("msg")
               .AddOption(new SlashCommandOptionBuilder()
                    .WithName("msg")
                    .WithDescription("msg")
                    .WithType(ApplicationCommandOptionType.String))
               .AddOption(new SlashCommandOptionBuilder()
                    .WithName("chan")
                    .WithDescription("chan")
                    .WithType(ApplicationCommandOptionType.Channel));
            var question = new SlashCommandBuilder()
                .WithName("question")
                .WithDescription("question");

            try
            {
                await guild.CreateApplicationCommandAsync(ping.Build());
                await guild.CreateApplicationCommandAsync(createRole.Build());
                await guild.CreateApplicationCommandAsync(giveRole.Build());
                await guild.CreateApplicationCommandAsync(removeRole.Build());
                await guild.CreateApplicationCommandAsync(powerLink.Build());
                await guild.CreateApplicationCommandAsync(powerMc.Build());
                await guild.CreateApplicationCommandAsync(powerStats.Build());
                await guild.CreateApplicationCommandAsync(powerStatsId.Build());
                await guild.CreateApplicationCommandAsync(powerBanner.Build());
                await guild.CreateApplicationCommandAsync(sendmsg.Build());
                await guild.CreateApplicationCommandAsync(question.Build());
            }
            catch (ApplicationCommandException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }
        #endregion
        #region slashHandler
        private async Task SlashHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "pingpower":
                    await HandlePingCommand(command);
                    break;
                case "powerrole":
                    await HandleCreateRoleCommand(command);
                    break;
                case "givepowerrole":
                    await HandleGiveRoleCommand(command);
                    break;
                case "removepowerrole":
                    await HandleRemoveRoleCommand(command);
                    break;
                case "powerlink":
                    await HandleLinkCommand(command);
                    break;
                case "powermc":
                    await HandleMCLinkCommand(command);
                    break;
                case "powerstats":
                    await HandleStatsCommand(command);
                    break;
                case "powerstatsid":
                    await HandleStatsIdCommand(command);
                    break;
                case "powerbanner":
                    await HandleBannerCommand(command);
                    break;
                case "pmsg":
                    await HandleMsgCommand(command);
                    break;
                case "question":
                    await HandleQuestionCommand(command);
                    break;
            }
        }
        private async Task HandleQuestionCommand(SocketSlashCommand command)
        {
            var User = (SocketGuildUser)command.User;
            var guild = User.Guild;
            var question = guild.GetRole(1112447163377668186);
            var chan = guild.GetTextChannel(1112446204144529549);
            if (User.Roles.Contains(question))
            {
                var res = new EmbedBuilder().WithTitle("Error").WithColor(Color.Red);
                string des = "You already have this role";
                res.Description = des;
                await command.RespondAsync(embed: res.Build(), ephemeral: true);
            }
            else
            {
                User.AddRoleAsync(question);
                var res = new EmbedBuilder().WithTitle("Success").WithColor(Color.Green);
                string des = "You have been given the question role!\n" +
                    "You can see the question at 12 EST in " + chan.Mention;
                res.Description = des;
                await command.RespondAsync(embed: res.Build(), ephemeral: true);
            }
        }
        private async Task HandleMsgCommand(SocketSlashCommand command)
        {
            string msg = command.Data.Options.ElementAt(0).Value.ToString();
            SocketTextChannel chan;
            var User = (SocketGuildUser)command.User;
            var guild = User.Guild;
            if (User.Id == 603000858161971211)
            {
                if(command.Data.Options.Count == 1)
                {
                    chan = command.Channel as SocketTextChannel;
                }
                else
                {
                    chan = command.Data.Options.ElementAt(1).Value as SocketTextChannel;
                }
                await chan.SendMessageAsync(msg);
            }
        }
        private async Task HandleBannerCommand(SocketSlashCommand command)
        {
            var User = (SocketGuildUser)command.User;
            var commandData = command.Data;
            var guild = User.Guild;
            SocketUser bUser = User;
            Boolean server = true;
            if (command.Data.Options.Count == 0)
            {
                bUser = User;
            }
            else
            {
                bUser = (SocketUser)command.Data.Options.ElementAt(0).Value;
            }
            var type = server ? "s" : "b";
            var banner = _commonService.getBanner(bUser, "b");
            await command.RespondAsync(embed: banner.Result.Build());
        }
        private async Task HandlePingCommand(SocketSlashCommand command)
        {
            await command.RespondAsync("pong");
        }
        private async Task HandleStatsCommand(SocketSlashCommand command)
        {
            var guildUser = (SocketGuildUser)command.User;
            var commandData = command.Data;
            var guild = guildUser.Guild;
            var validationResult = verify(command, commandData.Name.ToString());
            if (validationResult.Count() > 0)
            {
                var res = new EmbedBuilder().WithTitle("Errors").WithColor(Color.Red);
                string des = "";
                foreach (var e in validationResult)
                {
                    des += e.errorField + ": " + e.errorMessage + "\n";
                }
                res.Description = des;
                await command.RespondAsync(embed: res.Build(), ephemeral: true);
            }
            else
            {
                var tc = _dataService.getTimeCardId((long)guildUser.Id).Result;
                if (tc != null)
                {
                    var startTime = tc.startTime;
                    var end = DateTime.Now;
                    var dif = end - startTime;
                    var newUser = new UserActTransfer()
                    {
                        Id = (long)tc.userId,
                        TimeInVc = (long)dif.TotalMilliseconds
                    };
                    await _dataService.insertUserAct(newUser);
                    await _dataService.delTimeCard(newUser.Id);
                }
                var userName = guildUser.Nickname == null ? guildUser.Nickname : guildUser.Username;
                var embed = new EmbedBuilder().WithAuthor(guildUser).WithTitle("Power Stats").WithColor(Color.Magenta);
                var user = _dataService.getUserActId((long)guildUser.Id).Result;
                var time = TimeSpan.FromMilliseconds(user.TimeInVc);
                var timeWeek = TimeSpan.FromMilliseconds(user.TimeInVcWeek);
                var timeObj = time.Days.ToString() + "d " + time.Hours.ToString() + "h " + time.Minutes.ToString() + "m " + time.Seconds.ToString() + "." + time.Milliseconds.ToString() + "s";
                var weektimeObj = timeWeek.Days.ToString() + "d " + timeWeek.Hours.ToString() + "h " + timeWeek.Minutes.ToString() + "m " + timeWeek.Seconds.ToString() + "." + timeWeek.Milliseconds.ToString() + "s";
                var av = _dataService.getAve((long)guildUser.Id).Result;
                string des = "";
                if (guildUser.Roles.Contains(guild.GetRole(1086098983346241628)))
                {
                    var sho = _dataService.getsho((long)guildUser.Id).Result;
                    des = "Total interations: " + user.Count + " (" + av[0].ToString("0.00") + "%)"
                    + "\n Total pings: " + user.Mentions + " (" + av[1].ToString("0.00") + "%)"
                    + "\n Total images: " + user.Images + " (" + av[2].ToString("0.00") + "%)"
                    + "\n Total time in VC: " + timeObj + " (" + av[3].ToString("0.00") + "%)"
                    + "\n Total Sho pings: " + sho[0] + " (" + sho[1].ToString("0.00") + "%)"
                    + "\n Total interactions this week: " + user.WeekCount + " (" + av[4].ToString("0.00") + "%)"
                    + "\n Total pings this week: " + user.WeekMentions + " (" + av[5].ToString("0.00") + "%)"
                    + "\n Total images this week: " + user.WeekImages + " (" + av[6].ToString("0.00") + "%)"
                    + "\n Total time in VC this week: " + weektimeObj + " (" + av[7].ToString("0.00") + "%)";
                }
                else
                {
                    des = "Total interations: " + user.Count + " (" + av[0].ToString("0.00") + "%)"
                    + "\n Total pings: " + user.Mentions + " (" + av[1].ToString("0.00") + "%)"
                    + "\n Total images: " + user.Images + " (" + av[2].ToString("0.00") + "%)"
                    + "\n Total time in VC: " + timeObj + " (" + av[3].ToString("0.00") + "%)"
                    + "\n Total interactions this week: " + user.WeekCount + " (" + av[4].ToString("0.00") + "%)"
                    + "\n Total pings this week: " + user.WeekMentions + " (" + av[5].ToString("0.00") + "%)"
                    + "\n Total images this week: " + user.WeekImages + " (" + av[6].ToString("0.00") + "%)"
                    + "\n Total time in VC this week: " + weektimeObj + " (" + av[7].ToString("0.00") + "%)";
                }
                embed.Description = des;
                await command.RespondAsync(embed: embed.Build());
                if (guildUser.VoiceChannel != null)
                {
                    await _dataService.insertTimeCard(new TimeCardModel
                    {
                        startTime = DateTime.Now,
                        guildId = guild.Id,
                        userId = guildUser.Id
                    });
                }
            }
        }

        private async Task HandleStatsIdCommand(SocketSlashCommand command)
        {
            var guildUser = command.Data.Options.ElementAt(0).Value as SocketGuildUser;
            var commandData = command.Data;
            var guild = guildUser.Guild;
            var validationResult = verify(command, commandData.Name.ToString());
            if (validationResult.Count() > 0)
            {
                var res = new EmbedBuilder().WithTitle("Errors").WithColor(Color.Red);
                string des = "";
                foreach (var e in validationResult)
                {
                    des += e.errorField + ": " + e.errorMessage + "\n";
                }
                res.Description = des;
                await command.RespondAsync(embed: res.Build(), ephemeral: true);
            }
            else
            {
                var tc = _dataService.getTimeCardId((long)guildUser.Id).Result;
                if (tc != null)
                {
                    var startTime = tc.startTime;
                    var end = DateTime.Now;
                    var dif = end - startTime;
                    var newUser = new UserActTransfer()
                    {
                        Id = (long)tc.userId,
                        TimeInVc = (long)dif.TotalMilliseconds
                    };
                    await _dataService.insertUserAct(newUser);
                    await _dataService.delTimeCard(newUser.Id);
                }
                var userName = guildUser.Nickname == null ? guildUser.Nickname : guildUser.Username;
                var embed = new EmbedBuilder().WithAuthor(guildUser).WithTitle("Power Stats").WithColor(Color.Magenta);
                var user = _dataService.getUserActId((long)guildUser.Id).Result;
                var time = TimeSpan.FromMilliseconds(user.TimeInVc);
                var timeWeek = TimeSpan.FromMilliseconds(user.TimeInVcWeek);
                var timeObj = time.Days.ToString() + "d " + time.Hours.ToString() + "h " + time.Minutes.ToString() + "m " + time.Seconds.ToString() + "." + time.Milliseconds.ToString() + "s";
                var weektimeObj = timeWeek.Days.ToString() + "d " + timeWeek.Hours.ToString() + "h " + timeWeek.Minutes.ToString() + "m " + timeWeek.Seconds.ToString() + "." + timeWeek.Milliseconds.ToString() + "s";
                var av = _dataService.getAve((long)guildUser.Id).Result;
                string des = "";
                if (guildUser.Roles.Contains(guild.GetRole(1086098983346241628)))
                {
                    var sho = _dataService.getsho((long)guildUser.Id).Result;
                    des = "Total interations: " + user.Count + " (" + av[0].ToString("0.00") + "%)"
                    + "\n Total pings: " + user.Mentions + " (" + av[1].ToString("0.00") + "%)"
                    + "\n Total images: " + user.Images + " (" + av[2].ToString("0.00") + "%)"
                    + "\n Total time in VC: " + timeObj + " (" + av[3].ToString("0.00") + "%)"
                    + "\n Total Sho pings: " + sho[0] + " (" + sho[1].ToString("0.00") + "%)"
                    + "\n Total interactions this week: " + user.WeekCount + " (" + av[4].ToString("0.00") + "%)"
                    + "\n Total pings this week: " + user.WeekMentions + " (" + av[5].ToString("0.00") + "%)"
                    + "\n Total images this week: " + user.WeekImages + " (" + av[6].ToString("0.00") + "%)"
                    + "\n Total time in VC this week: " + weektimeObj + " (" + av[7].ToString("0.00") + "%)";
                }
                else
                {
                    des = "Total interations: " + user.Count + " (" + av[0].ToString("0.00") + "%)"
                    + "\n Total pings: " + user.Mentions + " (" + av[1].ToString("0.00") + "%)"
                    + "\n Total images: " + user.Images + " (" + av[2].ToString("0.00") + "%)"
                    + "\n Total time in VC: " + timeObj + " (" + av[3].ToString("0.00") + "%)"
                    + "\n Total interactions this week: " + user.WeekCount + " (" + av[4].ToString("0.00") + "%)"
                    + "\n Total pings this week: " + user.WeekMentions + " (" + av[5].ToString("0.00") + "%)"
                    + "\n Total images this week: " + user.WeekImages + " (" + av[6].ToString("0.00") + "%)"
                    + "\n Total time in VC this week: " + weektimeObj + " (" + av[7].ToString("0.00") + "%)";
                }
                embed.Description = des;
                await command.RespondAsync(embed: embed.Build());
                if (guildUser.VoiceChannel != null)
                {
                    await _dataService.insertTimeCard(new TimeCardModel
                    {
                        startTime = DateTime.Now,
                        guildId = guild.Id,
                        userId = guildUser.Id
                    });
                }
            }
        }

        private async Task HandleMCLinkCommand(SocketSlashCommand command)
         {
            var guildUser = (SocketGuildUser)command.User;
            var commandData = command.Data;
            var options = commandData.Options;
            string mcUserName = command.Data.Options.ElementAt(0).Value.ToString();
            var validationResult = verify(command, commandData.Name.ToString());
            if (validationResult.Count() > 0)
            {
                var res = new EmbedBuilder().WithTitle("Errors").WithColor(Color.Red);
                string des = "";
                foreach (var e in validationResult)
                {
                    des += e.errorField + ": " + e.errorMessage + "\n";
                }
                res.Description = des;
                await command.RespondAsync(embed: res.Build(), ephemeral: true);
            }
            else
            {
                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                {
                    client.BaseAddress = new Uri("https://api.mojang.com/users/profiles/minecraft/");
                    HttpResponseMessage response = client.GetAsync(mcUserName).Result;
                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        var result = await response.Content.ReadAsStreamAsync();
                        using var streamReader = new StreamReader(result);
                        using var jsonReader = new JsonTextReader(streamReader);
                        JsonSerializer serializer = new JsonSerializer();
                        var mcObj = serializer.Deserialize<McConnect>(jsonReader);
                        mcObj.discordId = (long)guildUser.Id;
                        var users = _commonService.GetMcUsersJson().Result;
                        users.Add(mcObj.Name);
                        await _commonService.updateMcUserJson(users);
                        await _dataService.insertMcUser(mcObj);
                        var suc = new EmbedBuilder().WithTitle("Success").WithColor(Color.Green);
                        string sucdes = "User has been MC Verified \n Join using play.power18.gg";
                        suc.Description = sucdes;
                        await command.Channel.SendMessageAsync(embed: suc.Build());
                    }
                    else
                    {
                        var res = new EmbedBuilder().WithTitle("Error").WithColor(Color.Red);
                        string des = "This username does not exist";
                        res.Description = des;
                        await command.Channel.SendMessageAsync(embed: res.Build());
                    }
                    
                }
            }
               
        }

        private async Task HandleLinkCommand(SocketSlashCommand command)
        {
            var guildUser = (SocketGuildUser)command.User;
            SocketGuildUser User = (SocketGuildUser)command.Data.Options.ElementAt(0).Value;
            SocketRole Role = (SocketRole)command.Data.Options.ElementAt(1).Value;
            var guild = guildUser.Guild;
            var commandData = command.Data;
            var options = commandData.Options;
            
            var validationResult = verify(command, commandData.Name.ToString());
            
            if (validationResult.Count() > 0)
            {
                var res = new EmbedBuilder().WithTitle("Errors").WithColor(Color.Red);
                string des = "";
                foreach (var e in validationResult)
                {
                    des += e.errorField + ": " + e.errorMessage + "\n";
                }
                res.Description = des;
                await command.RespondAsync(embed: res.Build(), ephemeral: true);
            }
            else
            {
                
                string userName = User.Nickname ?? User.Username;
                var existingRole = _dataService.doesUserHasRole((long)User.Id).Result;
                if(User.Roles.Contains(Role) && !existingRole)
                {
                    await _dataService.insertRole((long)User.Id, (long)Role.Id);
                }
                else
                {
                    await User.AddRoleAsync(Role);
                    await _dataService.insertRole((long)User.Id, (long)Role.Id);
                }
                
                var res = new EmbedBuilder().WithTitle(userName.ToString() + " successfully linked to role " + Role.Name.ToString()).WithColor(Color.Green);
                await command.RespondAsync(embed: res.Build(), ephemeral: true);

            }
        }
        private async Task HandleRemoveRoleCommand(SocketSlashCommand command)
        {
            var guildUser = (SocketGuildUser)command.User;
            SocketGuildUser newUser = (SocketGuildUser)command.Data.Options.ElementAt(0).Value;
            var guild = guildUser.Guild;
            var commandData = command.Data;
            var options = commandData.Options;
            var validationResult = verify(command, commandData.Name.ToString());
            if (validationResult.Count() > 0)
            {
                var res = new EmbedBuilder().WithTitle("Errors").WithColor(Color.Red);
                string des = "";
                foreach (var e in validationResult)
                {
                    des += e.errorField + ": " + e.errorMessage + "\n";
                }
                res.Description = des;
                await command.RespondAsync(embed: res.Build(), ephemeral: true);
            }
            else
            {
                var existingRole = guild.GetRole((ulong)_dataService.getUserHasRole((long)guildUser.Id).Result.RoleId);
                await newUser.RemoveRoleAsync(existingRole);
                string userName = newUser.Nickname ?? newUser.Username;
                var res = new EmbedBuilder().WithTitle(userName.ToString() + " successfully removed from role").WithColor(Color.Green);
                await command.RespondAsync(embed: res.Build(), ephemeral: true);
                
            }
        }
        private async Task HandleGiveRoleCommand(SocketSlashCommand command)
        {
            var guildUser = (SocketGuildUser)command.User;
            SocketGuildUser newUser = (SocketGuildUser)command.Data.Options.ElementAt(0).Value;
            var guild = guildUser.Guild;
            var commandData = command.Data;
            var options = commandData.Options;
            var validationResult = verify(command, commandData.Name.ToString());
            if (validationResult.Count() > 0)
            {
                var res = new EmbedBuilder().WithTitle("Errors").WithColor(Color.Red);
                string des = "";
                foreach (var e in validationResult)
                {
                    des += e.errorField + ": " + e.errorMessage + "\n";
                }
                res.Description = des;
                await command.RespondAsync(embed: res.Build(), ephemeral: true);
            }
            else
            {
                var existingRole = guild.GetRole((ulong)_dataService.getUserHasRole((long)guildUser.Id).Result.RoleId);
                await newUser.AddRoleAsync(existingRole);
                string userName = newUser.Nickname ?? newUser.Username;
                string ownerUserName = guildUser.Nickname ?? guildUser.Username;
                var res = new EmbedBuilder()
                    .WithTitle(newUser + " successfully given" + ownerUserName.ToString() + "\'s role!")
                    .WithColor(Color.Green);
                await command.RespondAsync("<@" + newUser.Id + "> \n", embed: res.Build());
            }
        }
        private async Task HandleCreateRoleCommand(SocketSlashCommand command)
        {
            var guildUser = (SocketGuildUser)command.User;
            SocketGuildUser roleOwner = (SocketGuildUser)command.Data.Options.ElementAt(1).Value;
            var guild = guildUser.Guild;
            var commandData = command.Data;
            var options = commandData.Options;
            var validationResult = verify(command, commandData.Name.ToString());
            if (validationResult.Count() > 0)
            {
                var res = new EmbedBuilder().WithTitle("Errors").WithColor(Color.Red);
                string des = "";
                foreach(var e in validationResult)
                {
                    des += e.errorField + ": " + e.errorMessage + "\n";
                }
                res.Description = des;
                await command.RespondAsync(embed: res.Build(), ephemeral: true);
            }
            else
            {
                var newRole = await guild.CreateRoleAsync(options.ElementAt(0).Value.ToString(), null, getColorfromHex(options.ElementAt(2).Value.ToString()).Result);
                await roleOwner.AddRoleAsync(newRole);
                await _dataService.insertRole((long)roleOwner.Id, (long)newRole.Id);
                var res = new EmbedBuilder().WithTitle("Role: " + newRole.Name + " created").WithColor(await getColorfromHex(command.Data.Options.ElementAt(2).Value.ToString()));
                await command.RespondAsync(embed: res.Build(), ephemeral: true);
            }
            //
        }
        private async Task<Discord.Color> getColorfromHex(string rawHex)
        {
            var hex = "0x" + rawHex.Substring(1);
            var c = new Discord.Color(Convert.ToUInt32(hex, 16));
            return c;
        }

        private List<VerificationErrors> verify(SocketSlashCommand command, string name)
        {
            var user = command.User as SocketGuildUser;
            var guild = user.Guild as SocketGuild;
            var errors = new List<VerificationErrors>();
            var modRole = 1086306852981190887;
            var roleOwner = 1106026221109784667;
            
            if(name == "powerrole")
            {
                SocketGuildUser roleFor = (SocketGuildUser)command.Data.Options.ElementAt(1).Value;
                var existingRole = _dataService.doesUserHasRole((long)roleFor.Id).Result;
                string hex = command.Data.Options.ElementAt(2).Value.ToString();
                
                if(existingRole)
                {
                    errors.Add(new VerificationErrors
                    {
                        errorField = "Existing role",
                        errorMessage = "This user already has a existing role"
                    });
                }

                if(!user.Roles.Any(x => x.Id == (ulong)(modRole)))
                {
                    errors.Add(new VerificationErrors
                    {
                        errorField = "Permissions",
                        errorMessage = "You do not have the required permissions"
                    });
                }

                if (!(Regex.IsMatch(hex.Substring(1), "[0-9A-Fa-f]+") && hex[0] == '#'))
                {
                    errors.Add(new VerificationErrors
                    {
                        errorField = "Color",
                        errorMessage = "Color must be a hex value"
                    });
                }
            }else if(name == "givepowerrole")
            {
                SocketGuildUser newUser = (SocketGuildUser)command.Data.Options.ElementAt(0).Value;
                var existingRoleOwner = _dataService.doesUserHasRole((long)user.Id).Result;
                var existingRole = _dataService.getUserHasRole((long)user.Id).Result;
                var existingRoleNew = newUser.Roles.Contains(guild.GetRole((ulong)existingRole.RoleId));
                if (!existingRoleOwner)
                {
                    errors.Add(new VerificationErrors
                    {
                        errorField = "Role",
                        errorMessage = "You do not have a custom role"
                    });
                }
                if (existingRoleNew)
                {
                    errors.Add(new VerificationErrors
                    {
                        errorField = "User",
                        errorMessage = "User is already in this role"
                    });
                }
            }
            else if (name == "removepowerrole")
            {
                SocketGuildUser newUser = (SocketGuildUser)command.Data.Options.ElementAt(0).Value;
                var existingRoleOwner = _dataService.doesUserHasRole((long)user.Id).Result;
                var existingRole = _dataService.getUserHasRole((long)user.Id).Result;
                var existingRoleNew = newUser.Roles.Contains(guild.GetRole((ulong)existingRole.RoleId));
                if (!existingRoleOwner)
                {
                    errors.Add(new VerificationErrors
                    {
                        errorField = "Role",
                        errorMessage = "You do not have a custom role"
                    });
                }
                if (!existingRoleNew)
                {
                    errors.Add(new VerificationErrors
                    {
                        errorField = "User",
                        errorMessage = "User in not in your role"
                    });
                }
                if (user == newUser)
                {
                    errors.Add(new VerificationErrors
                    {
                        errorField = "User",
                        errorMessage = "You cannot remove yourself from your role"
                    });
                }
            }else if(name == "powerlink")
            {
                SocketGuildUser newUser = (SocketGuildUser)command.Data.Options.ElementAt(0).Value;
                var existingRole = _dataService.getUserHasRole((long)newUser.Id).Result;

                if (user.GuildPermissions.Administrator != true)
                {
                    errors.Add(new VerificationErrors
                    {
                        errorField = "Permission",
                        errorMessage = "You must be an admin to use this command"
                    });
                }

                if (existingRole != null)
                {
                    var rolename = guild.GetRole((ulong)existingRole.RoleId);
                    errors.Add(new VerificationErrors
                    {
                        errorField = "Role",
                        errorMessage = "This person already has a custom role. Role Name: " + rolename
                    });
                }
            }else if(name == "powermc")
            {
                if (!user.Roles.Contains(guild.GetRole(1086128000694751252)))
                {
                    errors.Add(new VerificationErrors
                    {
                        errorField = "Role",
                        errorMessage = "You must be verified to join the minecraft server"
                    });
                }
                if (!_dataService.getMcUser((long)user.Id).Result)
                {
                    errors.Add(new VerificationErrors
                    {
                        errorField = "User",
                        errorMessage = "You have already registered an account, to remove it dm Haru"
                    });
                }
            }else if(name == "powerstats")
            {
                if(_dataService.getUserActId((long)user.Id).Result == null)
                {
                    errors.Add(new VerificationErrors
                    {
                        errorField = "User",
                        errorMessage = "You are not in the db"
                    });
                }
            }


            return errors;
        }
        #endregion
    }
}
