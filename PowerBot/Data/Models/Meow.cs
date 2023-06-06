using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBot.Data.Models
{
    [Table("Meow")]
    public class Meow
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("Count")]
        public int Count { get; set; }
    }
}
