using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBot.Data.Models
{
    [Table("UserRole")]
    public class UserRole
    {
        [Key]
        [Column("UserId")]
        public long UserId { get; set; }
        [Column("RoleId")]
        public long RoleId { get; set; }
    }
}
