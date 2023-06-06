using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBot.Data.Models
{
    [Table("TimeCard")]
    public class TimeCard
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("USER_NAME")]
        public string? Name { get; set; }

        [Column("USER_ID")]
        public long User_Id { get; set; }

        [Column("SERVER_ID")]
        public long Server_Id { get; set; }

        [Column("START_TIME")]
        public DateTime Start_Time { get; set; }
        [Column("CHANNEL_ID")]
        public long Channel_Id { get; set; }
        [Column("Type")]
        public int type { get; set; } = 2;
    }

}
