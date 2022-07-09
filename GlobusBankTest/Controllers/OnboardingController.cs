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
        private readonly IHelper helper;

        public OnboardingController(IOnboarding onboarding,
                                    IHashHelper hashHelper,
                                    IOtp otp, IMapper mapper,
                                    IConfiguration configuration,
                                    IHttpHelper http, IHelper helper)
        {
            this.onboarding = onboarding;
            this.hashHelper = hashHelper;
            this.otp = otp;
            this.mapper = mapper;
            this.configuration = configuration;
            this.http = http;
            this.helper = helper;
        }


        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DefaultResponse>> Register(RegisterDto register)
        {
            var response = new DefaultResponse();
            try
            {
                response = await this.helper.Register(register); ;
                if (response.IsSuccessful)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Value = ex.Message;
                return StatusCode(500, response);
            }


        }



        [HttpGet("getOtp/{phoneNumber}")]
        public async Task<ActionResult<DefaultResponse>> GetOtp(string phoneNumber)
        {
            var response = new DefaultResponse();
            try
            {
                response = await this.helper.GetOtp(phoneNumber); ;
                if (response.IsSuccessful)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Value = ex.Message;
                return StatusCode(500, response);
            }
        }

        [HttpPost("validateOtp")]
        public async Task<ActionResult<DefaultResponse>> ValidateOtp(GetOtpDto getOtp)
        {
            var response = new DefaultResponse();
            try
            {
                response = await this.helper.ValidateOtp(getOtp);
                if (response.IsSuccessful)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Value = ex.Message;
                return StatusCode(500, response);
            }


        }

        [HttpGet("getAll")]
        public async Task<ActionResult<DefaultResponse>> GetAllOnboardedUsers()
        {
            var response = new DefaultResponse();
            try
            {
                response = await this.helper.GetAllOnboardedUsers();
                if (response.IsSuccessful)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Value = ex.Message;
                return StatusCode(500, response);
            }

        }

        [HttpGet("getGoldPrice")]
        public async Task<ActionResult<DefaultResponse>> GetPriceOfGold()
        {
            var response = new DefaultResponse();
            try
            {
                response = await this.helper.GetPriceOfGold();
                if (response.IsSuccessful)
                {
                    return Ok(response);
                }
                return BadRequest(response);
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
