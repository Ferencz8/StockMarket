using StockMarket.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockMarket.Web.Services
{
    public interface IQuandlRequester
    {

        Task<IEnumerable<CompanyInfo>> GetCompanyInfos();

        Task<StockInformation> GetStockInfo(string code, DateTime? startDate, DateTime? endDate);
    }
}
