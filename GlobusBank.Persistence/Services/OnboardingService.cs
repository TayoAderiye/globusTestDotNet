using GlobusBank.Persistence.Interfaces;
using GlobusBank.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Persistence.Services
{
    public class OnboardingService : IOnboarding
    {
        private readonly OnboardingContext onboardingContext;

        public OnboardingService(OnboardingContext onboardingContext)
        {
            this.onboardingContext = onboardingContext;
        }


        public void CreateCommand(Onboarding onboarding)
        {
            try
            {
                if (onboarding == null)
                {
                    throw new ArgumentNullException(nameof(onboarding));
                }

                this.onboardingContext.Onboardings.Add(onboarding);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<IEnumerable<Onboarding>> GetAllOnboardings()
        {
            try
            {
                var data = await this.onboardingContext.Onboardings.ToListAsync();
                return data.Where(x => x.IsActive == true);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public bool SaveChanges()
        {
            try
            {
                return (this.onboardingContext.SaveChanges() >= 0);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public void UpdateCommand(Onboarding onboarding)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UserExits(string email, string phoneNumber)
        {
            try
            {
                return await this.onboardingContext.Onboardings.AnyAsync(x => x.Email == email.ToLower() || x.PhoneNumber == phoneNumber);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> UserExits(string phoneNumber)
        {
            try
            {
                return await this.onboardingContext.Onboardings.AnyAsync(x => x.PhoneNumber == phoneNumber);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<bool> UserIsActive(string phoneNumber)
        {
            try
            {
                return await this.onboardingContext.Onboardings.AnyAsync(x => x.PhoneNumber == phoneNumber && x.IsActive);
            }
            catch (Exception)
            {

                throw;
            }
   
        }

        public async Task<Onboarding> GetByPhoneNumber(string phoneNumber)
        {
            try
            {
                return await this.onboardingContext.Onboardings.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
