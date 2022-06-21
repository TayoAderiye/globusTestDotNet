using GlobusBank.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Persistence
{
    public class OnboardingContext: DbContext
    {
        public OnboardingContext(DbContextOptions<OnboardingContext> opt) : base(opt)
        {

        }

        public DbSet<Onboarding> Onboardings { get; set; }
        public DbSet<Otp> Otps { get; set; }
    }
}
