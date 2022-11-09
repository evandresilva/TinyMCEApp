using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace TinyMCEApp.Models
{
    public class LakeFsConfigModel
    {
        private readonly IConfiguration _configuration;
        //public string Token { get; set; }
        public string ClientId { get; set; }
        public string SecretKey { get; set; }
        public string Server { get; set; }
        public string Repository { get; set; }
        public string Branch { get; set; }
        public LakeFsConfigModel(IConfiguration configuration)
        {
            _configuration = configuration;
            //Token = _configuration.GetValue<string>("lakeFsToken");
            ClientId = _configuration.GetValue<string>("lakeFsClientId");
            SecretKey = _configuration.GetValue<string>("lakeFsSecreteKey");

            Server = _configuration.GetValue<string>("lakeFsServer");
            Branch = _configuration.GetValue<string>("lakeFsBranch");
            Repository = _configuration.GetValue<string>("lakeFsRepository");
        }
    }
}