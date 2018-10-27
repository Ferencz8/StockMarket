using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Platform.Web.Controllers
{
    public class StockController : Controller
    {
        private const string API_Key = "sXa9wicwVsB92p1BDMzH";
        private const string QuandlUrlFormat = @"https://www.quandl.com/api/v3/datatables/SHARADAR/SF1?ticker={0}&qopts.columns=ticker,revenue&api_key={1}";

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string GetData(string code)
        {
            string result = "";
            using (WebClient client = new WebClient())
            {
                var resultResponse = client.DownloadString(string.Format(QuandlUrlFormat, code, API_Key));
                JObject jObject = JObject.Parse(resultResponse);
                result = jObject["datatable"]["data"].ToString();
                
            }

            return result;
        }
    }
}