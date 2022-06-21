using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Persistence.Models
{
    public class Otp
    {
        [Key]
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
    }
}
