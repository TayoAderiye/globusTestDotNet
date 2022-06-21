using GlobusBank.Persistence.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Infrastructure.Interfaces
{
    public interface IHashHelper
    {
        HashPasswordsDto HashPassword(string password);
    }
}
