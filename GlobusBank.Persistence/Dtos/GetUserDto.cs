using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Persistence.Dtos
{
    public class GetUserDto
    {
        public string PhoneNumber { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string LGA { get; set; }
        public bool IsActive { get; set; }
    }
}
