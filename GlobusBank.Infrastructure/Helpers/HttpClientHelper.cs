using GlobusBank.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Infrastructure.Helpers
{
    public class HttpClientHelper : IHttpHelper
    {
        private readonly IConfiguration configuration;

        public HttpClientHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public RestResponse CallApi()
        {
            var url = this.configuration.GetSection("RapidUrl").Value;
            var key = this.configuration.GetSection("RapidKey").Value;
            var host = this.configuration.GetSection("RapidHost").Value;
            var client = new RestClient(url);
            var request = new RestRequest(Method.Get.ToString());
            request.AddHeader("X-RapidAPI-Key", key);
            request.AddHeader("X-RapidAPI-Host", host);
            RestResponse response = client.Execute(request);
            return response;
        }
    }
}
