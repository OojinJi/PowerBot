using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBot.Data.Models
{
    [Table("UserAct")]
    public class UserAct
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("Count")]
        public int Count { get; set; }
        [Column("Images")]
        public int? Images { get; set; }
        [Column("Mentions")]
        public int? Mentions { get; set; }

        [Column("TimeInVc")]
        public long? TimeInVc { get; set; }

        [Column("CountWeek")]
        public int WeekCount { get; set; }
        [Column("ImagesWeek")]
        public int? WeekImages { get; set; }
        [Column("MentionsWeek")]
        public int? WeekMentions { get; set; }

        [Column("TimeInVcWeek")]
        public long? TimeInVcWeek { get; set; }

        [Column("LastMsg")]
        public string? LastMsg { get; set; }


    }
}
