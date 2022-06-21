using AutoMapper;
using GlobusBank.Persistence.Dtos;
using GlobusBank.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Persistence.Profiles
{
    public class OnboardingProfile: Profile
    {
        public OnboardingProfile()
        {
            CreateMap<Onboarding, GetUserDto>();
        }
      
    }
}
