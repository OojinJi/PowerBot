using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBot.DataModel
{
    public class discordUserObj
    {
        public string id { get; set; }
        public string banner { get; set; }
        public discordUserObj user { get; set; }
    }
}
