using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerBot.Data;
using PowerBot.Data.Models;
using PowerBot.DataModel;
using PowerBot.Module;

namespace PowerBot.Services
{
    public class DataService
    {
        public async Task insertTimeCard(TimeCardModel timeCard)
        {
            using (var db = new PowerDbContext())
            {
                db.TimeCards.Add(new TimeCard
                {
                    User_Id = (long)timeCard.userId,
                    Server_Id = (long)timeCard.guildId,
                    Start_Time = timeCard.startTime,
                    Channel_Id = (long)timeCard.channelId
                });
                Console.WriteLine("Log: Timecard created");
                db.SaveChanges();
            }
        }
        public async Task<List<TimeCardModel>> getTimeCard()
        {
            using (var db = new PowerDbContext())
            {
                List<TimeCard> timeCard = db.TimeCards.Where(x => x.type == 2).ToList();
                List<TimeCardModel> timeCardList = new List<TimeCardModel>();
                foreach (var card in timeCard)
                {
                    timeCardList.Add(new TimeCardModel
                    {
                        userId = (ulong)card.User_Id,
                        guildId = (ulong)card.Server_Id,
                        startTime = card.Start_Time,
                        channelId = (ulong)card.Channel_Id
                    });
                }
                Console.WriteLine("Log: Timecards recieved");
                return timeCardList;
            }
        }
        public async Task<TimeCardModel> getTimeCardId(long Id)
        {
            using (var db = new PowerDbContext())
            {
                var ret = db.TimeCards.Where(x => x.User_Id == Id && x.Server_Id == 1086008152056672277 && x.type == 2).FirstOrDefault();
                if(ret == null)
                {
                    return null;
                }
                else
                {
                    var retVal = new TimeCardModel
                    {
                        userId = (ulong)ret.User_Id,
                        startTime = ret.Start_Time
                    };
                    Console.WriteLine("Log: Timecard of " + retVal.userId + " pulled");
                    return retVal;
                }
            }
        }
        public async Task delTimeCard(long id)
        {
            using (var db = new PowerDbContext())
            {
                db.TimeCards.Remove(db.TimeCards.FirstOrDefault(x => x.User_Id == id && x.Server_Id == 1086008152056672277 && x.type ==2) );
                db.SaveChanges();
                Console.WriteLine("Log: Time card of " + id + " has been deleted");
            }
        }
        public async Task<List<float>> getAve(long id)
        {
            List<float> av = new List<float>();
            using (var db = new PowerDbContext())
            {
                var user = db.UserActs.Where(x => x.Id == id).FirstOrDefault();
                float total = db.UserActs.Sum(x => x.Count);
                float totalI = (float)db.UserActs.Sum(x => x.Images);
                float totalM = (float)db.UserActs.Sum(x => x.Mentions);
                float totalvc = (float)db.UserActs.Sum(x => x.TimeInVc);
                float totalw = db.UserActs.Sum(x => x.WeekCount);
                float totalwi = (float)db.UserActs.Sum(x => x.WeekImages);
                float totalwm = (float)db.UserActs.Sum(x => x.WeekMentions);
                float totalwvc = (float)db.UserActs.Sum(x => x.TimeInVcWeek);
                var x = ((user.Count / total) * 100);
                av.Add((float)(user.Count/ total) * 100);
                av.Add((float)(user.Images/ totalI)*100);
                av.Add((float)(user.Mentions / totalM) * 100);
                av.Add((float)(user.TimeInVc / totalvc) * 100);
                av.Add((float)(user.WeekCount / totalw) * 100);
                av.Add((float)(user.WeekImages/ totalwi) * 100);
                av.Add((float)(user.WeekMentions / totalwm) * 100);
                av.Add((float)(user.TimeInVcWeek / totalwvc) * 100);
            }
            Console.WriteLine("Log: Stats for " + id + " have been pulled");
            return av;
        }
        public async Task<List<float>> getsho(long id)
        {
            List<float> av = new List<float>();
            using (var db = new PowerDbContext())
            {
                var u = db.ShoGirls.Where(x => x.Id == id).FirstOrDefault();
                if(u != null)
                {
                    float total = db.ShoGirls.Sum(x => x.Count);
                    av.Add((float)u.Count);
                    av.Add((float)(u.Count / total) * 100);
                    Console.WriteLine("Log: Returning Sho percentage for " + u.Name);
                }
                else
                {
                    av.Add(0);
                    av.Add(0);
                }
            }
            return av;
        }
        public async Task<List<UserActTransfer>> getUserAct(string t = "weekly")
        {
            var weekly = new List<UserActTransfer>();
            using (var db = new PowerDbContext())
            {
                if(t == "weekly")
                {
                    weekly.AddRange((db.UserActs.AsEnumerable().OrderByDescending(x => x.WeekCount).Take(3)).Select(x => new UserActTransfer { Id = x.Id, WeekCount = (int)x.WeekCount }));
                    Console.WriteLine("Log: Weekly activity leaderboard pulled");
                }
                else if(t == "all")
                {
                    weekly.AddRange((db.UserActs.AsEnumerable().OrderByDescending(x => x.Count).Take(3)).Select(x => new UserActTransfer { Id = x.Id, Count = x.Count}));
                    Console.WriteLine("Log: Lifetime activity leaderboard pulled");
                }
                return weekly;
            }
        }
        public async Task<List<UserActTransfer>> getUserVC(string t = "weekly")
        {
            var weekly = new List<UserActTransfer>();
            using (var db = new PowerDbContext())
            {
                if (t == "weekly")
                {
                    weekly.AddRange((db.UserActs.AsEnumerable().OrderByDescending(x => x.TimeInVcWeek).Take(3)).Select(x => new UserActTransfer { Id = x.Id, TimeInVcWeek = (int)x.TimeInVcWeek }));
                    Console.WriteLine("Log: Weekly voice activity leaderboard pulled");
                }
                else if (t == "all")
                {
                    weekly.AddRange((db.UserActs.AsEnumerable().OrderByDescending(x => x.TimeInVc).Take(3)).Select(x => new UserActTransfer { Id = x.Id, TimeInVc = (long)x.TimeInVc }));
                    Console.WriteLine("Log: Lifetime voice activity leaderboard pulled");
                }
                return weekly;
            }
        }
        public async Task insertMcUser(McConnect user)
        {
            using (var db = new PowerDbContext())
            {
                db.McConnects.Add(user);
                db.SaveChanges();
                Console.WriteLine("Log: New MC user added to db");
            }
        }
        public async Task<bool> getMcUser(long userId)
        {
            using(var db = new PowerDbContext())
            {
                Console.WriteLine("Log: Get MC user for " + userId);
                return db.McConnects.Where(x => x.discordId == userId).FirstOrDefault() == null ? true:false;
            }
        }
        public async Task WeeklyClear()
        {
            using (var db = new PowerDbContext())
            {
                foreach (var user in db.UserActs)
                {
                    user.WeekMentions = 0;
                    Console.WriteLine("Log: Weekly mentions set to 0");
                    user.WeekImages = 0;
                    Console.WriteLine("Log: Weekly images set to 0");
                    user.WeekCount = 0;
                    Console.WriteLine("Log: Weekly interactions set to 0");
                    user.TimeInVcWeek = 0;
                    Console.WriteLine("Log: Weekly time in vc set to 0");
                }
                db.SaveChanges();
            }
        }
        public async Task insertUserAct(UserActTransfer userA)
        {
            using (var db = new PowerDbContext())
            {
                var exit = db.UserActs.Where(x => x.Id == userA.Id).FirstOrDefault();
                if(exit == null)
                {
                    var newUser = new UserAct(){
                        Id = userA.Id,
                        Count = 1,
                        Images = userA.Images,
                        Mentions = userA.Mentions,
                        LastMsg = userA.LastMsg == null ? userA.LastMsg:" " ,
                        WeekImages = userA.Images > 0 ? userA.Images:0,
                        WeekMentions = userA.Mentions > 0 ? userA.Mentions : 0,
                        WeekCount = 1,
                        TimeInVc = userA.TimeInVc,
                        TimeInVcWeek = userA.TimeInVc,
                    };
                    db.UserActs.Add(newUser);
                    db.SaveChanges();
                    Console.WriteLine("Log: New user Activity added");
                }
                else if(exit.LastMsg != userA.LastMsg || userA.LastMsg == null)
                {
                    exit.Count += 1;
                    exit.Images += userA.Images;
                    exit.LastMsg = userA.LastMsg;
                    exit.Mentions += userA.Mentions;
                    exit.WeekCount += 1;
                    exit.WeekImages += userA.Images;
                    exit.WeekMentions += userA.Mentions;
                    exit.TimeInVc += userA.TimeInVc;
                    exit.TimeInVcWeek += userA.TimeInVc;
                    db.SaveChanges();
                    Console.WriteLine("Log: User Activity updated");
                }
                db.SaveChanges();
            }
        }
        public async Task<UserActTransfer> getUserActId(long id)
        {
            using (var db = new PowerDbContext())
            {
                var exit = db.UserActs.Where(x => x.Id == id).FirstOrDefault();
                if(exit == null)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine("Log: Pulling stats of " + id);
                    return new UserActTransfer() {
                        Id = exit.Id,
                        Count = exit.Count,
                        Images = (int)exit.Images,
                        Mentions = (int)exit.Mentions,
                        WeekCount = exit.WeekCount,
                        WeekImages = (int)exit.WeekImages,
                        WeekMentions = (int)exit.WeekMentions,
                        LastMsg = exit.LastMsg,
                        TimeInVc = (long)exit.TimeInVc,
                        TimeInVcWeek= (long)exit.TimeInVcWeek
                    };
                }
            }
        }
        public async Task resetTime()
        {
            using (var db = new PowerDbContext())
            {
                foreach(var userAct in db.UserActs)
                {
                    userAct.TimeInVc = 0;
                    userAct.TimeInVcWeek = 0;
                }
                Console.WriteLine("Log: Weekly VC time set to 0");
                db.SaveChanges();
            }
        }
        public async Task<List<string>> cleanUsers(SocketGuild guild)
        {
            List<string> users = new List<string>();
            using (var db = new PowerDbContext())
            {
                foreach (var meows in db.Meow)
                {
                    if(guild.GetUser((ulong)meows.Id) == null)
                    {
                        db.Meow.Remove(meows);
                        users.Add("<@" + meows.Id + "> cleaned from meows");
                    }
                }
                foreach (var nw in db.NWords)
                {
                    if (guild.GetUser((ulong)nw.Id) == null)
                    {
                        db.NWords.Remove(nw);
                        users.Add("<@" + nw.Id + "> cleaned from something");
                    }
                }
                foreach (var ur in db.UserRoles)
                {
                    if (guild.GetUser((ulong)ur.UserId) == null)
                    {
                        db.UserRoles.Remove(ur);
                        users.Add("<@" + ur.UserId + "> cleaned from Roles");
                    }
                }
                foreach (var ur in db.UserActs)
                {
                    if (guild.GetUser((ulong)ur.Id) == null)
                    {
                        db.UserActs.Remove(ur);
                        users.Add("<@" + ur.Id + "> cleaned from activity");
                    }
                }
                Console.WriteLine("Log: Users cleaned");
                db.SaveChanges();
            }
            return users;
        }
        public async Task<int> meowCount()
        {
            using (var db = new PowerDbContext())
            {
                Console.WriteLine("Log: Pulling meow count");
                return db.Meow.Sum(x => x.Count);
            }
        }

        public async Task insertBestBoi(List<string> urls)
        {
            using (var db = new PowerDbContext())
            {
                foreach (string url in urls)
                {
                    db.BestBoi.Add(new BestBoi
                    {
                        Url = url
                    });
                    db.SaveChanges();
                }
            }
        }
        public async Task<string> getBestBoi(int num)
        {
            using (var db = new PowerDbContext())
            {
                return db.BestBoi.Where(x => x.Number == num).FirstOrDefault().Url;
            }
        }
        public async Task<int> getBestBoiCount()
        {
            using (var db = new PowerDbContext())
            {
                return db.BestBoi.Count();
            }
        }
        public async Task insertBestGirl(List<string> urls)
        {
            using (var db = new PowerDbContext())
            {
                foreach (string url in urls)
                {
                    db.BestGirl.Add(new BestGirl
                    {
                        Url = url
                    });
                    db.SaveChanges();
                }
            }
        }
        public async Task<string> getBestGirl(int num)
        {
            using (var db = new PowerDbContext())
            {
                return db.BestGirl.Where(x => x.Number == num).FirstOrDefault().Url;
            }
        }
        public async Task<int> getBestGirlCount()
        {
            using (var db = new PowerDbContext())
            {
                return db.BestGirl.Count();
            }
        }
        public async Task insertSelfie(List<string> urls)
        {
            using (var db = new PowerDbContext())
            {
                foreach (string url in urls)
                {
                    db.Selfies.Add(new Selfie
                    {
                        Url = url
                    });
                    db.SaveChanges();
                }
            }
        }
        public async Task<string> getSelfie(int num)
        {
            using (var db = new PowerDbContext())
            {
                return db.Selfies.Where(x => x.Number == num).FirstOrDefault().Url;
            }
        }
        public async Task<int> getSelfieCount()
        {
            using (var db = new PowerDbContext())
            {
                return db.Selfies.Count();
            }
        }
        public async Task<int> InMeow(SocketGuildUser user)
        {
            using (var db = new PowerDbContext())
            {
                var exit = db.Meow.Where(x => (ulong)x.Id == user.Id).FirstOrDefault();
                if (exit == null)
                {
                    db.Meow.Add(new Meow
                    {
                        Id = (long)user.Id,
                        Count = 1
                    });
                    Console.WriteLine("Log: New Kitten added to db");
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    exit.Count++;
                    Console.WriteLine("Log: Kitten updated to db");
                    db.SaveChanges();
                    return exit.Count + 1;
                }
            }
        }
        public async Task NwordCount(SocketGuildUser user)
        {
            using (var db = new PowerDbContext())
            {
                var exit = db.NWords.Where(x => (ulong)x.Id == user.Id).FirstOrDefault();
                if (exit == null)
                {
                    db.NWords.Add(new NWord
                    {
                        Id = (long)user.Id,
                        Count = 1
                    });
                    db.SaveChanges();
                }
                else
                {
                    exit.Count++;
                    db.SaveChanges();
                }
            }
        }
        public async Task<List<Meow>> MeowList()
        {
            using (var db = new PowerDbContext())
            {
                return (db.Meow.AsEnumerable().OrderByDescending(x => x.Count).ToList());
            }
        }
        public async Task<List<NWord>> NWordList()
        {
            using (var db = new PowerDbContext())
            {
                return (db.NWords.AsEnumerable().OrderByDescending(x => x.Count).ToList());
            }
        }
        public async Task<string> insertRole(long userId, long RoleId)
        {
            using (var db = new PowerDbContext())
            {
                var exits = db.UserRoles.Where(x => x.UserId == userId).FirstOrDefault();
                if (exits == null)
                {
                    db.UserRoles.Add(new UserRole
                    {
                        UserId = userId,
                        RoleId = RoleId
                    });
                    Console.WriteLine("Log: New role created for" + userId);
                    db.SaveChanges();
                    return "success";
                }
                return null;
            }
        }
        public async Task<bool> doesUserHasRole(long userId)
        {
            using (var db = new PowerDbContext())
            {
                var s = db.UserRoles.Where(x => x.UserId == userId).FirstOrDefault() != null;
                return s;
            }
        }
        public async Task<UserRole> getUserHasRole(long userId)
        {
            using (var db = new PowerDbContext())
            {
                var s = db.UserRoles.Where(x => x.UserId == userId).FirstOrDefault();
                return s;

            }
        }
        public async void deleteRole(long userId)
        {
            using(var db = new PowerDbContext())
            {
                db.UserRoles.Remove(db.UserRoles.Where(x => x.UserId == userId).FirstOrDefault());
                Console.WriteLine("Log: Role of user " + userId + " has been removed");
            }
        }
    }
}
