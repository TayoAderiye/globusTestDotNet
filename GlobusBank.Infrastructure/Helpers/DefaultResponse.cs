using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Infrastructure.Helpers
{
    public class DefaultResponse
    {
        public bool IsSuccessful { get; set; } = false;
        public bool IsFailure => !IsSuccessful;
        public object Value { get; set; }
        public int StatusCode { get; set; }

    }
}
