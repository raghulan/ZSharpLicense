using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSharpLicHelper.APIEntities
{
    public class Licresponse
    {
        public string status { get; set; }
        public string data { get; set; }
        public string seat_id { get; set; }
        public string expired_on { get; set; }
        public string interval { get; set; }
    }

    public class LicInfo
    {
        public int responseCode { get; set; }
        public string responseStatus { get; set; }
        public string responseMessage { get; set; }
        public Licresponse response { get; set; }
    }

    public class Response
    {
        public string customer_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public string fax { get; set; }
        public object cart { get; set; }
        public object wishlist { get; set; }
        public string newsletter { get; set; }
        public string address_id { get; set; }
        public string custom_field { get; set; }
        public string status { get; set; }
        public string safe { get; set; }
        public string token { get; set; }
        public string code { get; set; }
        public string date_added { get; set; }
        public LicInfo lic_info { get; set; }
    }

    public class MainResponse
    {
        public int responseCode { get; set; }
        public string responseStatus { get; set; }
        public string responseMessage { get; set; }
        public Response response { get; set; }
    }

    public class LogOutResponse
    {
        public string status { get; set; }
        public string data { get; set; }
    }

    public class LogOutMainResponse
    {
        public int responseCode { get; set; }
        public string responseStatus { get; set; }
        public string responseMessage { get; set; }
        public LogOutResponse response { get; set; }
    }
}
