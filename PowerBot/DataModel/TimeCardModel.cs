using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBot.DataModel
{
    public class TimeCardModel
    {
        public DateTime startTime { get; set; }
        public ulong userId { get; set; }
        public ulong channelId { get; set; }
        public ulong guildId { get; set; }
        public double time { get; set; }
        public string userName { get; set; }
        public int type { get; set; } = 2;
    }
}
