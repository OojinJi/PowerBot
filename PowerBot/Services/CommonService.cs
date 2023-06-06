using Discord;
using Discord.Audio;
using Discord.WebSocket;
using FluentFTP;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PowerBot.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentFTP.GnuTLS;
using FluentFTP.GnuTLS.Enums;
using PowerBot.Module;

namespace PowerBot.Services
{
    public class CommonService
    {
        private readonly DataService _dataService;
        private readonly DiscordSocketClient _discord;
        private bool trigged = false;
        private bool off = false;
        private int rnd = 1;
        private readonly string whitelist = "whitelist.json";
        public CommonService(DataService dataService, IServiceProvider services)
        {
            _dataService = dataService;
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _discord.MessageReceived += MeowAsync;
            _discord.UserLeft += cleanUser;
            _discord.LatencyUpdated += heartBeat;
            _discord.UserVoiceStateUpdated += voiceEvent;
            
            _discord.Ready += startTime;
        }


        
        public async Task startTime()
        {
            Console.WriteLine("Log: Starting startup VC check");
            var cards = _dataService.getTimeCard().Result;
            var guild = _discord.GetGuild(1086008152056672277);
            foreach(var user in cards)
            {
                var gUser = guild.GetUser(user.userId);
                if(gUser.VoiceChannel == null && !gUser.IsBot)
                {
                    var startTime = user.startTime;
                    var end = DateTime.Now;
                    var dif = end - startTime;
                    var newUser = new UserActTransfer()
                    {
                        Id = (long)gUser.Id,
                        TimeInVc = (long)dif.TotalMilliseconds
                    };
                    await _dataService.insertUserAct(newUser);
                    await _dataService.delTimeCard(newUser.Id);
                }
            }
            var invC = guild.Users.Where(x => x.VoiceChannel != null).ToList();
            foreach(var user in invC)
            {
                var card = _dataService.getTimeCardId((long)user.Id).Result;
                if(card == null && !user.IsBot)
                {
                    await _dataService.insertTimeCard(new TimeCardModel
                    {
                        startTime = DateTime.Now,
                        guildId = guild.Id,
                        userId = user.Id
                    });
                }
            }
            Console.WriteLine("Log: Startup VC check complete");
        }

        public async Task heartBeat(int x, int y)
        {

            TimeSpan lower = new TimeSpan(23, 59, 00);
            TimeSpan upper = new TimeSpan(23, 59, 59);
            TimeSpan rlower = new TimeSpan(00, 00, 00);
            TimeSpan rupper = new TimeSpan(00, 59, 59);
            DateTime now = DateTime.Now;
            var rand = new Random();
            int beat = rand.Next(0, 4999);
            if(beat == rnd)
            {
                var guild = _discord.GetGuild(1086008152056672277);
                if(guild != null)
                {
                    var randNum = rand.Next(1, _dataService.getSelfieCount().Result);
                    var chan = guild.GetTextChannel(1086129901200998411);
                    var url = _dataService.getSelfie(randNum).Result;
                    await chan.SendMessageAsync(url);
                    Console.WriteLine("Log: Random selfie sent");
                }
            }
            if ((now.DayOfWeek == DayOfWeek.Sunday && ((now.TimeOfDay >= lower) && (now.TimeOfDay <= upper))) && trigged == false)
            {
                await leader();
                trigged = true;
                await _dataService.WeeklyClear();
            }
            if ((now.DayOfWeek == DayOfWeek.Monday && ((now.TimeOfDay >= lower) && (now.TimeOfDay <= upper))) && trigged == true)
            {
                trigged = false;
            }
        }

        public async Task leader()
        {
            Console.WriteLine("Sending weekly server statictics...");
            var guild = _discord.GetGuild(1086008152056672277);
            var chan = guild.GetTextChannel(1107796280937295962);
            var invC = guild.Users.Where(x => x.VoiceChannel != null).ToList();
            foreach (var user in invC)
            {
                var tc = _dataService.getTimeCardId((long)user.Id).Result;
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
            }
            List<UserActTransfer> leaderAll = _dataService.getUserAct("all").Result;
            string des2 = "";
            int counter2 = 1;
            var embed2 = new EmbedBuilder()
            {
                Title = "The most active members in this server are:"
            };
            embed2.WithColor(new Color(0xffaed7));
            if (leaderAll.Count > 0)
            {
                foreach (UserActTransfer u in leaderAll)
                {
                    if (u.Count != 0)
                    {
                        SocketUser curUser = guild.GetUser((ulong)u.Id);
                        var user = (_dataService.getAve(u.Id)).Result;
                        des2 += (counter2.ToString() + ". " + curUser.Mention + " has interacted in the server " + u.Count + " times (" + user[0] + ")\n");
                        counter2++;
                        if (counter2 - 1 == 3)
                        {
                            break;
                        }
                    }
                }
                if (des2 != "")
                {
                    embed2.WithDescription(des2);
                }
                else
                {
                    embed2.WithDescription("Not enough data");
                }
                Console.WriteLine("Log: Sending all time Active member");
                var msg2 = await chan.SendMessageAsync(embed: embed2.Build());
            }

            List<UserActTransfer> leaderVc = _dataService.getUserVC("all").Result;
            string desvc = "";
            int countervc = 1;
            var embedvc = new EmbedBuilder()
            {
                Title = "The people who have spent the most time in vc are:"
            };
            embedvc.WithColor(new Color(0xffaed7));
            if (leaderVc.Count > 0)
            {
                foreach (UserActTransfer u in leaderVc)
                {
                    var user = (_dataService.getAve(u.Id)).Result;
                    if (u.TimeInVc != 0)
                    {
                        SocketUser curUser = guild.GetUser((ulong)u.Id);
                        var time = TimeSpan.FromMilliseconds(u.TimeInVc);
                        var timeObj = time.Days.ToString() + "d " + time.Hours.ToString() + "h " + time.Minutes.ToString() + "m " + time.Seconds.ToString() + "." + time.Milliseconds.ToString() + "s";
                        desvc += (countervc.ToString() + ". " + curUser.Mention + " has been in vc for " + timeObj + " (" + user[4] + ")\n");
                        countervc++;
                        if (countervc - 1 == 3)
                        {
                            break;
                        }
                    }
                }
                if (desvc != "")
                {
                    embedvc.WithDescription(desvc);
                }
                else
                {
                    embedvc.WithDescription("Not enough data");
                }
                Console.WriteLine("Log: Sending all time VC activity");
                var msg = await chan.SendMessageAsync(embed: embedvc.Build());
            }

            List<UserActTransfer> leader = _dataService.getUserAct("weekly").Result;
            string des = "";
            int counter = 1;
            var embed = new EmbedBuilder()
            {
                Title = "The most active members in this server for the week of " + DateOnly.FromDateTime(DateTime.Today) + " are:"
            };
            embed.WithColor(new Color(0xffaed7));
            if (leader.Count > 0)
            {
                foreach (UserActTransfer u in leader)
                {
                    var user = (_dataService.getAve(u.Id)).Result;
                    if (u.WeekCount != 0)
                    {
                        SocketUser curUser = guild.GetUser((ulong)u.Id);

                        des += (counter.ToString() + ". " + curUser.Mention + " has interacted in the server " + u.WeekCount + " times (" + user[4] + ")\n");
                        counter++;
                        if (counter - 1 == 3)
                        {
                            break;
                        }
                    }
                }
                if (des != "")
                {
                    embed.WithDescription(des);
                }
                else
                {
                    embed.WithDescription("Not enough data");
                }
                Console.WriteLine("Log: Sending weekly activity leaderboard");
                var msg = await chan.SendMessageAsync(embed: embed.Build());
            }

            List<UserActTransfer> leaderwVc = _dataService.getUserVC("weekly").Result;
            string deswvc = "";
            int counterwvc = 1;
            var embedwvc = new EmbedBuilder()
            {
                Title = "The people who have spent the most time in vc week of " + DateOnly.FromDateTime(DateTime.Today) + " are:"
            };
            embedwvc.WithColor(new Color(0xffaed7));
            if (leaderwVc.Count > 0)
            {
                foreach (UserActTransfer u in leaderwVc)
                {
                    var user = (_dataService.getAve(u.Id)).Result;
                    if (u.TimeInVcWeek!= 0)
                    {
                        SocketUser curUser = guild.GetUser((ulong)u.Id);
                        var timeWeek = TimeSpan.FromMilliseconds(u.TimeInVcWeek);
                        var weektimeObj = timeWeek.Days.ToString() + "d " + timeWeek.Hours.ToString() + "h " + timeWeek.Minutes.ToString() + "m " + timeWeek.Seconds.ToString() + "." + timeWeek.Milliseconds.ToString() + "s";
                        deswvc += (countervc.ToString() + ". " + curUser.Mention + " has been in vc for " + weektimeObj + " (" + user[7] + ")\n");
                        counterwvc++;
                        if (counterwvc - 1 == 3)
                        {
                            break;
                        }
                    }
                }
                if (desvc != "")
                {
                    embedwvc.WithDescription(deswvc);
                }
                else
                {
                    embedwvc.WithDescription("Not enough data");
                }
                Console.WriteLine("Log: Sending weekly VC activity");
                var msg = await chan.SendMessageAsync(embed: embedwvc.Build());
            }
            foreach (var user in invC)
            {
                var tc = _dataService.getTimeCardId((long)user.Id).Result;
                if (tc != null)
                {
                    await _dataService.insertTimeCard(new TimeCardModel
                    {
                        startTime = DateTime.Now,
                        guildId = guild.Id,
                        userId = user.Id
                    });
                }
            }

        }
        public void uploadftp()
        {
            var ftpclient = new FtpClient("us.nyc-01.redlinehosting.net:5024", "oojjiinn.fe860d13", "Pc5$5$5$Watcher");
            ftpclient.Config.CustomStream = typeof(GnuTlsStream);
            ftpclient.Config.CustomStreamConfig = new GnuConfig()
            {
                SecuritySuite = GnuSuite.Secure128
            };
            ftpclient.Connect();
            ftpclient.UploadFile(@"whitelist.txt", "/plugins/Whitelist/whitelist.txt");
            Console.WriteLine("Log: Whitelist updated");
            ftpclient.Disconnect();
        }
        public async Task<EmbedBuilder> getBanner(SocketUser user, string type = null)
        {
            SocketUser gUser = user;
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                Stream stream = null;
                if (type == null || type == "s")
                {
                    client.BaseAddress = new Uri("https://discord.com/api/v10/guilds/1086008152056672277/members/");
                    client.DefaultRequestHeaders.Add("Authorization", "Bot " + Environment.GetEnvironmentVariable("PowerBotToken", EnvironmentVariableTarget.Machine));
                    HttpResponseMessage response = client.GetAsync(gUser.Id.ToString()).Result;
                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        stream = await response.Content.ReadAsStreamAsync();
                        Console.WriteLine("Log: Server banner aquired");
                    }
                    else
                    {
                        Console.WriteLine("Log: error: " + response.StatusCode.ToString());
                    }
                }
                else if (type == "b")
                {
                    client.BaseAddress = new Uri("https://discord.com/api/v10/users/");
                    client.DefaultRequestHeaders.Add("Authorization", "Bot " + Environment.GetEnvironmentVariable("PowerBotToken", EnvironmentVariableTarget.Machine));
                    HttpResponseMessage response = client.GetAsync(gUser.Id.ToString()).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        stream = await response.Content.ReadAsStreamAsync();
                        Console.WriteLine("Log: Base banner aquired");
                    }
                    else
                    {
                        Console.WriteLine("Log: error: " + response.StatusCode.ToString());
                    }
                }
                using var streamReader = new StreamReader(stream);
                using var jsonReader = new JsonTextReader(streamReader);
                JsonSerializer serializer = new JsonSerializer();
                var userObj = serializer.Deserialize<discordUserObj>(jsonReader);
                var emb = new EmbedBuilder();
                if (userObj.banner == null && userObj.user.banner == null)
                {
                    emb.WithAuthor(gUser);
                    emb.WithTitle("User Does not have a banner");
                    emb.WithColor(Color.Red);
                    Console.WriteLine("Log: error: User does not have a banner");
                }
                else
                {
                    var format = ".png";
                    var bannerraw = userObj.user.banner != null ? userObj.user.banner : userObj.banner;
                    var userid = userObj.user.banner != null ? userObj.user.id : userObj.id;
                    var title = userObj.user.banner != null ? "Server Banner" : "User Banner";
                    if (bannerraw.Substring(0,2) == "a_")
                    {
                        format = ".gif";
                    }
                    var banner = "https://cdn.discordapp.com/banners/" + userid + "/" + bannerraw + format + "?size=512";
                    emb.WithAuthor(gUser);
                    emb.WithTitle(title);
                    emb.WithImageUrl(banner);
                    emb.WithColor(Color.Magenta);
                    Console.WriteLine("Log: Banner sent");
                }
                return emb;
            }
        }
        public async Task voiceEvent(SocketUser user, SocketVoiceState curVoiceState, SocketVoiceState nextVoiceState)
        {
            var gUser = user as SocketGuildUser;
            var guild = _discord.GetGuild(1086008152056672277);

            if (!gUser.IsBot)
            {
                if ((curVoiceState.VoiceChannel == null || curVoiceState.VoiceChannel.Id == 1086179518168973422) && (nextVoiceState.VoiceChannel != null))
                {
                    await _dataService.insertTimeCard(new TimeCardModel
                    {
                        startTime = DateTime.Now,
                        guildId = guild.Id,
                        userId = user.Id
                    });
                    Console.WriteLine("Log:" + user.Username + ": has joined a vc");
                }
                if (curVoiceState.VoiceChannel != null && (nextVoiceState.VoiceChannel == null || nextVoiceState.VoiceChannel.Id == 1086179518168973422))
                {
                    var card = _dataService.getTimeCardId((long)user.Id).Result;
                    if (card != null)
                    {
                        var startTime = card.startTime;
                        var end = DateTime.Now;
                        var dif = end - startTime;
                        var newUser = new UserActTransfer()
                        {
                            Id = (long)user.Id,
                            TimeInVc = (long)dif.TotalMilliseconds
                        };
                        await _dataService.insertUserAct(newUser);
                        await _dataService.delTimeCard(newUser.Id);
                        Console.WriteLine("Log: " + user.Username + ": has left the vc after" + dif);
                    }
                }
            }
        }

        public async Task<List<string>> GetMcUsersJson()
        {
            using StreamReader reader = new(whitelist);
            var json = reader.ReadToEnd();
            List<string> users = File.ReadAllLines(@"whitelist.txt").ToList();
            if (users.Count == 0)
            {
                List<string> newlist = new List<string>();
                return newlist;
            }
            Console.WriteLine("Log: Reading MC whitelist.txt");
            return users;  
        }

        public async Task updateMcUserJson(List<string> users)
        {
            using (StreamWriter writer = new StreamWriter(@"whitelist.txt"))
            {
                foreach(string user in users)
                {
                    writer.WriteLine(user);
                    Console.WriteLine("Log: adding" + user + " to whitelist");
                }
            }
            uploadftp();

        }
        public async Task cleanUser(SocketGuild guild, SocketUser user)
        {
            await _dataService.cleanUsers(guild);
        }
        public async Task MeowAsync(SocketMessage rawMessage)
        {
            var msgType = rawMessage.Channel.GetType();

            if ((msgType.Name == "SocketDMChannel") )
            {
                if ((rawMessage.Author.Id == 757682870725771316))
                {
                    List<String> fileUrl = new List<string>();
                    foreach (var a in rawMessage.Attachments)
                    {
                        fileUrl.Add(a.Url);
                    }
                    Console.WriteLine("Log: Image of Albeion recived");
                    await _dataService.insertBestBoi(fileUrl);
                }
                else if ((rawMessage.Author.Id == 622480881474600971))
                {
                    List<String> fileUrl = new List<string>();
                    foreach (var a in rawMessage.Attachments)
                    {
                        fileUrl.Add(a.Url);
                    }
                    Console.WriteLine("Log: Image of Athena recived");
                    await _dataService.insertBestGirl(fileUrl);
                }else if(rawMessage.Author.Id == 603000858161971211)
                {
                    List<String> fileUrl = new List<string>();
                    foreach (var a in rawMessage.Attachments)
                    {
                        fileUrl.Add(a.Url);
                    }
                    Console.WriteLine("Log: Selfie recived");
                    await _dataService.insertSelfie(fileUrl);
                }
            }
            else
            {
                var message = rawMessage as SocketUserMessage;
                    var msg = (from c in message.Content
                           where c != ' '
                           select c).Distinct();

                var newMsg = string.Join("", msg).ToLower();
                var chan = message.Channel as SocketGuildChannel;
                if (message.Reference != null && message.Content.Contains("ur done") && (message.Author as SocketGuildUser).GuildPermissions.Administrator)
                {
                    var guild = (message.Channel as SocketGuildChannel).Guild;
                    var ogMsg = message.ReferencedMessage;
                    var ogAuth = ogMsg.Author as SocketGuildUser;
                    await ogAuth.SetTimeOutAsync(new TimeSpan(0, 0, 1, 0));
                    await ogMsg.DeleteAsync();
                }
                if(message.Content.ToLower().Contains("is it raining?"))
                {
                    Console.WriteLine("Log: ig I'm gary today");
                    await message.ReplyAsync("IT NOT RAINING");
                }

                if((chan.Id == 1086165212664705065 
                    || chan.Id == 1086129225016283216 
                    || chan.Id == 1086129836235423754 
                    || chan.Id == 1086129901200998411
                    || chan.Id == 1086130090120839189
                    || chan.Id == 1086308888317534218
                    || chan.Id == 1088480558457294858
                    || chan.Id == 1086859302293213245
                    || chan.Id == 1088479327034818680
                    || chan.Id == 1089808466195009536
                    || chan.Id == 1086178222812696596
                    || chan.Id == 1086178248846753792
                    || chan.Id == 1086178421584957461
                    || chan.Id == 1086178291678990376
                    || chan.Id == 1086178184564842556
                    || chan.Id == 1090061346093142108
                    || chan.Id == 1086178767308853308
                    || chan.Id == 1086303018766577815
                    || chan.Id == 1086302596718923836) && !message.Author.IsBot)
                {
                    var newUser = new UserActTransfer()
                        {
                            Id = (long)message.Author.Id,
                            Mentions = message.MentionedUsers.Count,
                            Images = message.Attachments.Count,
                            LastMsg = message.Content
                        };
                    Console.WriteLine("Log: message received");
                    await _dataService.insertUserAct(newUser);
                }
                if((message.Author.Id == 332508914694356992 || message.Author.Id == 603000858161971211) && message.Content.Contains("@violet's ghost gang"))
                {
                    message.ReplyAsync((message.Channel as SocketGuildChannel).Guild.GetRole(1089056210256404593).Mention);
                }
                if (chan.Guild.Id == 1086008152056672277 && (newMsg.Contains("meow") || newMsg.Contains("nyah") || newMsg.Contains("nya") || newMsg.Contains("cat") || newMsg.Contains("kitten")))
                {
                    int mCount = _dataService.InMeow(message.Author as SocketGuildUser).Result;
                    Console.WriteLine(message.Author.Username + " moewed");

                    if(mCount % 10000 == 0)
                    {
                       await message.Channel.SendMessageAsync(message.Author.Mention + ", you have meowed for the " + mCount + "th time!!");
                    }
                }
                else if (chan.Guild.Id == 1086008152056672277 && (newMsg.Contains("niga") || newMsg.Contains("nigar")))
                {
                    await _dataService.NwordCount(message.Author as SocketGuildUser);
                    Console.WriteLine(message.Author.Username + " said the n-word");
                }
            }
        }
    }
}
