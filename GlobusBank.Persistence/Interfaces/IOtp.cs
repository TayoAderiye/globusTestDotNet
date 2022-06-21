using GlobusBank.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Persistence.Interfaces
{
    public interface IOtp
    {
        bool SaveChanges();
        void CreateOtp(Otp otp);
        Task<Otp> GetOtp(string phoneNumber);
        //Otp ValidateOtp(string phoneNumber, string code);
    }
}
