using CsvHelper;
using Newtonsoft.Json.Linq;
using StockMarket.Web.Models;
using StockMarket.Web.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockMarket.Web
{
    public class QuandlRequester : IQuandlRequester
    {
        private const string API_Key = "sXa9wicwVsB92p1BDMzH";
        private const string QuandlCodesUrl = "https://s3.amazonaws.com/quandl-production-static/end_of_day_us_stocks/ticker_list.csv";
        private const string QuandlUrlFormat = @"https://www.quandl.com/api/v3/datasets/WIKI/{0}.json?column_index=1&order=asc&api_key={1}&start_date={2}&end_date={3}";


        public async Task<IEnumerable<CompanyInfo>> GetCompanyInfos()
        {
            List<CompanyInfo> companies = new List<CompanyInfo>();
            using (HttpClient client = new HttpClient())
            {
                using (Stream response = await client.GetStreamAsync(QuandlCodesUrl))
                {
                    using (TextReader reader = new StreamReader(response))
                    {
                        using (var csvReader = new CsvReader(reader))
                        {
                            csvReader.Read();
                            while (csvReader.Read())
                            {
                                string code, name;
                                csvReader.TryGetField<string>(0, out code);
                                csvReader.TryGetField<string>(2, out name);

                                companies.Add(new CompanyInfo() { Code = code, Name = name });
                            }
                        }
                    }
                }
            }
            return companies;
        }

        public async Task<StockInformation> GetStockInfo(string code, DateTime? startDateParam, DateTime? endDateParam)
        {
            var startDate = (startDateParam ?? DateTime.Now.AddYears(-1)).ToString("yyyy-MM-dd");
            var endDate = (endDateParam ?? DateTime.Now).ToString("yyyy-MM-dd");
            var url = string.Format(QuandlUrlFormat, code, API_Key, startDate, endDate);

            StockInformation stockInfo = new StockInformation();
            stockInfo.StartDate = DateTime.Parse(startDate);
            stockInfo.EndDate = DateTime.Parse(endDate);
            stockInfo.Code = code;

            using (HttpClient client = new HttpClient())
            {
                var resultResponse = await client.GetStringAsync(url);
                JObject jObject = JObject.Parse(resultResponse);
                stockInfo.Data = jObject.SelectToken("$..data").ToString();
                stockInfo.Description = jObject.SelectToken("$..name").ToString();
            }
            return stockInfo;
        }
    }
}
