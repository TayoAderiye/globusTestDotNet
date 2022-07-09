using GlobusBank.Infrastructure.Helpers;
using GlobusBank.Persistence.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Infrastructure.Interfaces
{
    public interface IHelper
    {
        Task<DefaultResponse> Register(RegisterDto register);
        Task<DefaultResponse> GetOtp(string phoneNumber);
        Task<DefaultResponse> ValidateOtp(GetOtpDto getOtp);
        Task<DefaultResponse> GetAllOnboardedUsers();
        Task<DefaultResponse> GetPriceOfGold();
    }
}
