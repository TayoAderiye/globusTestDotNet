using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Persistence.Dtos
{
    public class GetOtpDto
    {
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
    }
}
