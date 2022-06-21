using GlobusBank.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Persistence.Interfaces
{
    public interface IOnboarding
    {
        bool SaveChanges();
        Task<IEnumerable<Onboarding>> GetAllOnboardings();
        void CreateCommand(Onboarding onboarding);
        void UpdateCommand(Onboarding onboarding);
        Task<bool> UserExits(string email, string phoneNumber);
        Task<bool> UserExits(string phoneNumber);
        Task<bool> UserIsActive(string phoneNumber);
        Task<Onboarding> GetByPhoneNumber(string phoneNumber);
    }
}
