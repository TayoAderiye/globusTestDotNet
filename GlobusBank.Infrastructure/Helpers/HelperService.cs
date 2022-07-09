using AutoMapper;
using GlobusBank.Infrastructure.Interfaces;
using GlobusBank.Persistence.Dtos;
using GlobusBank.Persistence.Interfaces;
using GlobusBank.Persistence.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Infrastructure.Helpers
{
    public class HelperService : IHelper
    {
        private readonly IOnboarding onboarding;
        private readonly IHashHelper hashHelper;
        private readonly IOtp otp;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IHttpHelper http;
        public HelperService(IOnboarding onboarding,
                                    IHashHelper hashHelper,
                                    IOtp otp, IMapper mapper,
                                    IConfiguration configuration,
                                    IHttpHelper http)
        {
            this.onboarding = onboarding;
            this.hashHelper = hashHelper;
            this.otp = otp;
            this.mapper = mapper;
            this.configuration = configuration;
            this.http = http;
        }
        public async Task<DefaultResponse> Register(RegisterDto register)
        {
            var response = new DefaultResponse();
            try
            {
                if (await EmailOrPhoneNumberExists(register.Email, register.PhoneNumber))
                {
                    response.IsSuccessful = false;
                    response.Value = $"User exists";
                    response.StatusCode = 400;
                    return response;
                }
                HashPasswordsDto data = this.hashHelper.HashPassword(register.Password);
                var path = Path.Combine(Environment.CurrentDirectory, @"Data\lga.json");
                var lgas = await JsonHelper.LoadJson<List<LgaDto>>(path);
                var checkerState = lgas.Find(x => x.State.ToLower() == register.State.ToLower());
                if (checkerState == null)
                {
                    response.IsSuccessful = false;
                    response.Value = $"This state doesn't exists";
                    response.StatusCode = 400;
                    return response; 
                }
                var checkerLga = checkerState.Lgas.Contains(register.LGA.ToLower());
                if (!checkerLga)
                {
                    response.IsSuccessful = false;
                    response.Value = $"LGA doesn't belong to this state";
                    response.StatusCode = 400;
                    return response;
                }
                var user = new Onboarding
                {
                    PhoneNumber = register.PhoneNumber,
                    Email = register.Email,
                    PasswordHash = data.PasswordHash,
                    PasswordSalt = data.PasswordSalt,
                    State = register.State,
                    LGA = register.LGA,
                };
                this.onboarding.CreateCommand(user);
                this.onboarding.SaveChanges();

                var generatedCode = RandomNumeric();
                var otp = new Otp
                {
                    Code = generatedCode,
                    PhoneNumber = register.PhoneNumber,
                };
                this.otp.CreateOtp(otp);
                this.otp.SaveChanges();
                //var request = HttpContext.Request;

                response.IsSuccessful = true;
                response.Value = $"User {user.Email} has been created successfully, Please get OTP localhost:5001/api/onboarding/getOtp/{user.PhoneNumber}";
                response.StatusCode = 200;
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = 500;
                response.Value = e.Message;
                return response;
            }
        }


        public async Task<DefaultResponse> GetOtp(string phoneNumber)
        {
            var response = new DefaultResponse();
            try
            {
                var data = await this.otp.GetOtp(phoneNumber);
                if (data == null) 
                {
                    response.StatusCode = 400;
                    response.IsSuccessful = false;
                    response.Value = $"OTP not Found";
                    return response;
                }
                //check if user if already active
                var checker = await this.onboarding.GetByPhoneNumber(phoneNumber);
                if (checker.IsActive)
                {
                    
                   response.StatusCode = 400;
                   response.Value = "User Otp has already been validated";
                }
                response.StatusCode = 200;
                response.IsSuccessful = true;
                response.Value = $"Your OTP is {data.Code}";
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = 500;
                response.Value = e.Message;
                return response;
            }
            
        }

        public async Task<DefaultResponse> ValidateOtp(GetOtpDto getOtp)
        {
            var response = new DefaultResponse();
            try
            {
                var user = await this.onboarding.GetByPhoneNumber(getOtp.PhoneNumber);
                if (user == null)
                {
                    response.StatusCode = 400;
                    response.IsSuccessful = false;
                    response.Value = $"User does'nt exists";
                    return response;
                }
                if (user.IsActive)
                {
                    response.StatusCode = 400;
                    response.IsSuccessful = false;
                    response.Value = $"OTP has been already been validated";
                    return response;
                }
                //check if code is what is on the db\
                var checkOtp = await this.otp.GetOtp(getOtp.PhoneNumber);
                if (checkOtp.Code != getOtp.Code)
                {
                    response.StatusCode = 400;
                    response.IsSuccessful = false;
                    response.Value = $"OTP is wrong";
                    return response;
                }

                user.IsActive = true;
                this.onboarding.SaveChanges();
                response.StatusCode = 200;
                response.IsSuccessful = true;
                response.Value = $"User Validated";
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = 500;
                response.Value = e.Message;
                return response;
            }
        }

        public async Task<DefaultResponse> GetAllOnboardedUsers()
        {
            var response = new DefaultResponse();
            try
            {
                var data = await this.onboarding.GetAllOnboardings();
                var mappedData = mapper.Map<IEnumerable<GetUserDto>>(data);
                response.StatusCode = 200;
                response.IsSuccessful = true;
                response.Value = mappedData;
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = 500;
                response.Value = e.Message;
                return response;
            }
        }

        public async Task<DefaultResponse> GetPriceOfGold()
        {
            var response = new DefaultResponse();
            try
            {
                var apiResponse = this.http.CallApi();
                if (apiResponse.Content == null)
                {
                    response.StatusCode = 400;
                    response.IsSuccessful = false;
                    response.Value = "Unable to connect to remote server";
                    return response;
                }
                var content = JsonConvert.DeserializeObject<GoldDto>(apiResponse.Content);
                if (apiResponse.IsSuccessful)
                {
                    response.StatusCode = 200;
                    response.IsSuccessful = true;
                    response.Value = content;
                    return response;
                }
                response.StatusCode = 400;
                response.IsSuccessful = false;
                response.Value = "Error";
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = 500;
                response.Value = e.Message;
                return response;
            }
        }

        private async Task<bool> EmailOrPhoneNumberExists(string email, string phoneNumber)
        {
            return await this.onboarding.UserExits(email, phoneNumber);
        }

        private static string RandomNumeric()
        {
            Random res = new Random();
            String str = "0123456789";
            int size = 5;
            String randomstring = "";
            for (int i = 0; i < size; i++)
            {
                int x = res.Next(str.Length);
                randomstring = randomstring + str[x];
            }
            return randomstring;
        }

      
    }
}
