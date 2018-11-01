using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Platform.Web.Models;
using StockMarket.Web.Models;
using StockMarket.Web.Services;

namespace Platform.Web.Controllers
{
    public class StockController : Controller
    {
        private const string CompanyList = "CompanyList";
        
        
        private readonly IMemoryCache _memoryCache;
        private readonly IQuandlRequester _quandlRequester;

        public StockController(IMemoryCache memoryCache, IQuandlRequester quandlRequester)
        {
            this._memoryCache = memoryCache;
            this._quandlRequester = quandlRequester;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetCompanies(string searchCompany)
        {
            var companies = await _memoryCache.GetOrCreateAsync<IEnumerable<CompanyInfo>>(CompanyList, entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromHours(1));

                return _quandlRequester.GetCompanyInfos();
            });

            var filteredCompanies = companies.Where(c => c.Name.ToLower().Contains(searchCompany.ToLower()));
            return Json(filteredCompanies);
        }

        [HttpGet]
        public async Task<JsonResult> GetData(string code, DateTime? startDateParam, DateTime? endDateParam)
        {
            var stockInfo = await _quandlRequester.GetStockInfo(code, startDateParam, endDateParam);
            return Json(stockInfo);
        }

        [HttpGet]
        public ActionResult GetStockCard(string code, string description)
        {
            return PartialView("StockCard", new StockCardViewModel() { Code = code, Description = description });
        }
    }
}