using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBot.Data.Models
{
    [Table("McConnect")]
    public class McConnect
    {
        [Key]
        [Column("DiscordId")]
        public long discordId { get; set; }
        [Column("UUID")]
        public string Id { get; set; }
        [Column("McUser")]
        public string Name { get; set; }
    }
}
