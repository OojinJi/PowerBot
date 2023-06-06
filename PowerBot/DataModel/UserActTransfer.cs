using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBot.DataModel
{
    public class UserActTransfer
    {
        public long Id { get; set; }
        public int Count { get; set; }
        public int Images { get; set; }
        public int Mentions { get; set; }
        public long TimeInVc { get; set; }
        public int WeekCount { get; set; }
        public int WeekImages { get; set; }
        public int WeekMentions { get; set; }
        public long TimeInVcWeek { get; set; }
        public string LastMsg { get; set; }
    }
}
