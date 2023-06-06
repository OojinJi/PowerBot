using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBot.Data.Models
{
    [Table("bestBoi")]
    public class BestBoi
    {
        [Key]
        [Column("Number")]
        public int Number { get; set; }
        [Column("url")]
        public string Url { get; set; }
    }
}
