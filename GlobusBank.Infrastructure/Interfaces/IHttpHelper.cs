using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Infrastructure.Interfaces
{
    public interface IHttpHelper
    {
        RestResponse CallApi();
    }
}
