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
    public class OtpService : IOtp
    {
        private readonly OnboardingContext onboardingContext;

        public OtpService(OnboardingContext onboardingContext)
        {
            this.onboardingContext = onboardingContext;
        }
        public void CreateOtp(Otp otp)
        {
            try
            {
                if (otp == null)
                {
                    throw new ArgumentNullException(nameof(otp));
                }

                this.onboardingContext.Otps.Add(otp);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<Otp> GetOtp(string phoneNumber)
        {
            try
            {
                return await this.onboardingContext.Otps.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
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
    }
}
