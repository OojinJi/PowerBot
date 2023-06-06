using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PowerBot.Data.Models;
using System.Net;


using PowerBot.Services;
using Newtonsoft.Json;
using PowerBot.DataModel;

namespace PowerBot.Module
{
    public class Comands : ModuleBase<SocketCommandContext>
    {
        private readonly CommonService _commonService;
        private readonly DataService _dataService;
        private readonly GrabberService _grabberService;

        public Comands(CommonService commonService, DataService dataService, GrabberService grabberService)
        {
            _commonService = commonService;
            _dataService = dataService;
            _grabberService = grabberService;   
        }


        [Command("test")]
        public async Task test()
        {
            await _commonService.leader();
        }
        [Command("banner")]
        public async Task banner(SocketUser _user = null)
        {
            SocketUser gUser;
            if (_user == null)
            {
                gUser = Context.User;
            }
            else
            {
                gUser = _user;
            }
            var banner = _commonService.getBanner(gUser, "b");
            await Context.Channel.SendMessageAsync(embed: banner.Result.Build());
        }


        [Command("fun")]
        public async Task fun()
        {
            if (Context.User.Id == 603000858161971211)
            {

                var bill = new EmbedBuilder()
                {
                    Title = "Bill"
                };
                bill.Color = Color.Orange;
                var body = new EmbedBuilder()
                {
                    Title = "Body"
                };
                body.Color = Color.Teal;
                var feet = new EmbedBuilder()
                {
                    Title = "Feet"
                };
                feet.Color = Color.Orange;
                var hat = new EmbedBuilder()
                {
                    Title = "Hat"
                };
                hat.Color = Color.DarkOrange;

                await Context.Message.ReplyAsync(embed: bill.Build());
                await Context.Channel.SendMessageAsync(embed: body.Build());
                await Context.Channel.SendMessageAsync(embed: feet.Build());
                await Context.Channel.SendMessageAsync("Wait who are you?");
                await Context.Channel.SendMessageAsync("An embeded text stack?");
                await Context.Channel.SendMessageAsync(embed: hat.Build());
                await Context.Channel.SendMessageAsync(embed: bill.Build());
                await Context.Channel.SendMessageAsync(embed: body.Build());
                await Context.Channel.SendMessageAsync(embed: feet.Build());
                await Context.Channel.SendMessageAsync("PERRY THE EMBEDED TEXT STACK!!!!");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Sorry only my owner can send this command");
            }
        }

        [Command("bestBoi")]
        public async Task bestBoi(string i = "1")
        {
            int num = _dataService.getBestBoiCount().Result;
            int paramNum = int.Parse(i);
            if(paramNum > num)
            {
                await Context.Message.ReplyAsync("There are not enough pictures to send");
                return;
            }
            else
            {
                if (int.Parse(i) < 5)
                {
                    for (int x = 0; x < 5; x++)
                    {
                        var rand = new Random();
                        var randNum = rand.Next(1, num);
                        var url = _dataService.getBestBoi(randNum).Result;
                        await Context.Channel.SendMessageAsync(url);
                    }
                }
                else
                {
                    var rand = new Random();
                    var randNum = rand.Next(1, num);
                    var url = _dataService.getBestBoi(randNum).Result;
                    await Context.Channel.SendMessageAsync(url);
                }

            }
        }

        [Command("bestGirl")]
        public async Task bestGirl(string i = "1")
        {

            int num = _dataService.getBestGirlCount().Result;
            int paramNum = int.Parse(i);
            if (paramNum > num)
            {
                await Context.Message.ReplyAsync("There are not enough pictures to send");
                return;
            }
            else
            {
                if(int.Parse(i) < 5)
                {
                    for (int x = 0; x < paramNum; x++)
                    {
                        var rand = new Random();
                        var randNum = rand.Next(1, num);
                        var url = _dataService.getBestGirl(randNum).Result;
                        await Context.Channel.SendMessageAsync(url);
                    }
                }
                else
                {
                    var rand = new Random();
                    var randNum = rand.Next(1, num);
                    var url = _dataService.getBestBoi(randNum).Result;
                    await Context.Channel.SendMessageAsync(url);
                }
            }
        }
        [Command("clean")]
        public async Task cleanUser()
        {
            var user = _dataService.cleanUsers(Context.Guild).Result;
            if(user.Count > 0)
            {
                foreach(var u in user)
                {
                    await Context.Channel.SendMessageAsync(u);
                } 
            }
        }

        [Command("who is the biggest kitten?")]
        [Alias("bestKitten?")]
        [Summary("Gets the top N results for server you are currently in")]
        public async Task MeowLeaderBoard(int num = 5)
        {
            SocketGuild curGuild = Context.Guild;
            List<Meow> leaderBoard = await _dataService.MeowList();
            string des = "";
            int counter = 1;

            var embed = new EmbedBuilder()
            {
                Title = "Biggest kitten in " + curGuild.Name
            };
            embed.WithColor(new Color(0xffaed7));
            if (leaderBoard.Count > 0)
            {
                foreach (Meow l in leaderBoard)
                {
                    if (l.Count != 0)
                    {
                        SocketUser curUser = curGuild.GetUser((ulong)l.Id);
                        des += (counter.ToString() + ". " + curUser.Mention + " has meowed " + l.Count + " time(s)\n");
                        counter++;
                        if(counter-1 == num)
                        {
                            break;
                        }
                    }
                }
                if (des != "")
                {
                    embed.WithDescription(des);
                    embed.WithFooter("This server has meowed " + _dataService.meowCount().Result + " time(s)");
                }
                else
                {
                    embed.WithDescription("Not enough data");
                }
                await Context.Message.ReplyAsync(embed: embed.Build());

            }
        }
    }
}
