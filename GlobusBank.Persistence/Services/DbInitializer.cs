using GlobusBank.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Persistence.Services
{
    public class DbInitializer: IDbInitializer
    {
        private readonly OnboardingContext onboardingContext;

        public DbInitializer(OnboardingContext onboardingContext)
        {
            this.onboardingContext = onboardingContext;
        }

        public void Initialize()
        {
            try
            {
                if (this.onboardingContext.Database.GetPendingMigrations().Any())
                {
                    this.onboardingContext.Database.Migrate();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
