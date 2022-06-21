using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Persistence.Dtos
{
    public class LgaDto
    {
        public string State { get; set; }
        public string Alias { get; set; }
        public List<String> Lgas { get; set; }
    }
}
