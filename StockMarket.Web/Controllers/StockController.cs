using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Platform.Web.Models;
using StockMarket.Web.Models;

namespace Platform.Web.Controllers
{
    public class StockController : Controller
    {
        private const string API_Key = "sXa9wicwVsB92p1BDMzH";
        //private const string QuandlUrlFormat = @"https://www.quandl.com/api/v3/datatables/SHARADAR/SF1?ticker={0}&qopts.columns=ticker,revenue&api_key={1}";
        private const string QuandlUrlFormat = @"https://www.quandl.com/api/v3/datasets/WIKI/{0}.json?column_index=1&api_key={1}&start_date={2}&end_date={3}";
        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //public string GetData(string code)
        //{
        //    string result = "";
        //    using (WebClient client = new WebClient())
        //    {
        //        var resultResponse = client.DownloadString(string.Format(QuandlUrlFormat, code, API_Key));
        //        JObject jObject = JObject.Parse(resultResponse);
        //        result = jObject["datatable"]["data"].ToString();

        //    }

        //    return result;
        //}

        [HttpGet]
        public JsonResult GetData(string code, DateTime? startDateParam, DateTime? endDateParam)
        {
            var startDate = (startDateParam ?? DateTime.Now.AddYears(-1)).ToString("yyyy-MM-dd");
            var endDate = (endDateParam ?? DateTime.Now).ToString("yyyy-MM-dd");
            var url = string.Format(QuandlUrlFormat, code, API_Key, startDate, endDate);

            StockInformation stockInfo = new StockInformation();
            stockInfo.StartDate = DateTime.Parse(startDate);
            stockInfo.EndDate = DateTime.Parse(endDate);
            stockInfo.Code = code;

            using (WebClient client = new WebClient())
            {
                var resultResponse = client.DownloadString(url);
                JObject jObject = JObject.Parse(resultResponse);
                stockInfo.Data = jObject.SelectToken("$..data").ToString();
                stockInfo.Description = jObject.SelectToken("$..name").ToString();
            }

            return Json(stockInfo);
        }

        public ActionResult GetStockCard(string code, string description)
        {
            return PartialView("StockCard", new StockCardViewModel() { Code = code, Description = description });
        }
    }
}