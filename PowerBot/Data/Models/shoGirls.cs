using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBot.Data.Models
{
    [Table("Girls")]
    public class shoGirls
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("count")]
        public int Count { get; set; }
        [Column("timesToday")]
        public int timeToday { get; set; }
        [Column("date")]
        public DateTime? Date { get; set; }
    }

}
