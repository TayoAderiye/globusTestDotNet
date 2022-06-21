using AutoMapper;
using GlobusBank.Infrastructure.Helpers;
using GlobusBank.Infrastructure.Interfaces;
using GlobusBank.Persistence.Dtos;
using GlobusBank.Persistence.Interfaces;
using GlobusBank.Persistence.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GlobusBankTest.Controllers
{
    [Route("api/onboarding")]
    [ApiController]
    public class OnboardingController : Controller
    {
        private readonly IOnboarding onboarding;
        private readonly IHashHelper hashHelper;
        private readonly IOtp otp;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IHttpHelper http;

        public OnboardingController(IOnboarding onboarding,
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


        [HttpPost("register")]
        public async Task<ActionResult<DefaultResponse>> Register(RegisterDto register)
        {
            try
            {
                if (await EmailOrPhoneNumberExists(register.Email, register.PhoneNumber)) return StatusCode((int)HttpStatusCode.BadRequest, new DefaultResponse { IsSuccessful = false, Value = $"User exists" });
                HashPasswordsDto data = this.hashHelper.HashPassword(register.Password);
                var path = Path.Combine(Environment.CurrentDirectory, @"Data\lga.json");
                var lgas = await JsonHelper.LoadJson<List<LgaDto>>(path);
                var checkerState = lgas.Find(x => x.State.ToLower() == register.State.ToLower());
                if (checkerState == null) return StatusCode((int)HttpStatusCode.BadRequest, new DefaultResponse { IsSuccessful = false, Value = $"This state doesn't exists" });
                var checkerLga = checkerState.Lgas.Contains(register.LGA.ToLower());
                if (!checkerLga)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new DefaultResponse { IsSuccessful = false, Value = $"LGA doesn't belong to this state" });
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
                var requestUrl = $"{Request.Scheme}://{Request.Host.Value}/";
                return StatusCode((int)HttpStatusCode.Created, new DefaultResponse
                {
                    IsSuccessful = true,
                    Value = $"User {user.Email} has been created successfully, Please get OTP {requestUrl}api/onboarding/getOtp/{user.PhoneNumber}",

                });
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new DefaultResponse
                {
                    IsSuccessful = false,
                    Value = $"{e.Message}",
                });
                throw;
            }


        }



        [HttpGet("getOtp/{phoneNumber}")]
        public async Task<ActionResult<DefaultResponse>> GetOtp(string phoneNumber)
        {
            try
            {
                var data = await this.otp.GetOtp(phoneNumber);
                if (data == null) return StatusCode((int)HttpStatusCode.NotFound, new DefaultResponse
                {
                    IsSuccessful = false,
                    Value = $"OTP not Found"

                });
                //check if user if already active
                var checker = await this.onboarding.GetByPhoneNumber(phoneNumber);
                if (checker.IsActive) return StatusCode((int)HttpStatusCode.BadRequest, new DefaultResponse { IsSuccessful = false, Value = "User Otp has already been validated" });
                return StatusCode((int)HttpStatusCode.OK, new DefaultResponse
                {
                    IsSuccessful = true,
                    Value = $"Your OTP is {data.Code}",
                });
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new DefaultResponse
                {
                    IsSuccessful = false,
                    Value = $"{e.Message}",
                });
                throw;
            }
        }

        [HttpPost("validateOtp")]
        public async Task<ActionResult<DefaultResponse>> ValidateOtp(GetOtpDto getOtp)
        {
            try
            {
                var user = await this.onboarding.GetByPhoneNumber(getOtp.PhoneNumber);
                if (user == null) return StatusCode((int)HttpStatusCode.BadRequest, new DefaultResponse
                {
                    IsSuccessful = false,
                    Value = $"User does'nt exists"

                });
                if (user.IsActive)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new DefaultResponse
                    {
                        IsSuccessful = false,
                        Value = $"OTP has been already been validated"

                    });
                }
                //check if code is what is on the db\
                var checkOtp = await this.otp.GetOtp(getOtp.PhoneNumber);
                if (checkOtp.Code != getOtp.Code)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new DefaultResponse
                    {
                        IsSuccessful = false,
                        Value = $"OTP is wrong"

                    });
                }
                user.IsActive = true;
                this.onboarding.SaveChanges();

                return StatusCode((int)HttpStatusCode.OK, new DefaultResponse
                {
                    IsSuccessful = true,
                    Value = $"User Validated"

                });
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new DefaultResponse
                {
                    IsSuccessful = false,
                    Value = $"{e.Message}",
                });
                throw;
            }


        }

        [HttpGet("getAll")]
        public async Task<ActionResult<DefaultResponse>> GetAllOnboardedUsers()
        {
            try
            {
                var data = await this.onboarding.GetAllOnboardings();
                var mappedData = mapper.Map<IEnumerable<GetUserDto>>(data);
                return StatusCode((int)HttpStatusCode.OK, new DefaultResponse
                {
                    IsSuccessful = true,
                    Value = mappedData
                });
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new DefaultResponse
                {
                    IsSuccessful = false,
                    Value = $"{e.Message}",
                });
                throw;
            }

        }

        [HttpGet("getGoldPrice")]
        public async Task<ActionResult<DefaultResponse>> GetPriceOfGold()
        {
            try
            {
                //var apiResponse = await CallRestApi(_verifyMeBaseUrl, verifyCacUrl, content, apiSecret, "POST");
                var apiResponse = this.http.CallApi();
                if (apiResponse.Content == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new DefaultResponse
                    {
                        IsSuccessful = true,
                        Value = "Unable to connect to remote server"
                    });
                }
                var content = JsonConvert.DeserializeObject<GoldDto>(apiResponse.Content);
                if (apiResponse.IsSuccessful)
                {
                    return StatusCode((int)HttpStatusCode.OK, new DefaultResponse
                    {
                        IsSuccessful = true,
                        Value = content
                    });
                }
                return StatusCode((int)HttpStatusCode.BadRequest, new DefaultResponse
                {
                    IsSuccessful = false,
                    Value = "Error"
                });
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new DefaultResponse
                {
                    IsSuccessful = false,
                    Value = $"{e.Message}",
                });
                throw;
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
