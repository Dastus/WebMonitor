using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;

namespace Monitor.Application.Interfaces
{
    public interface IHttpRequestService
    {
        Task<HtmlDocument> GetHtmlStructure(string url);
        Task<WebPageLoadResult> GetPageLoadResult(string url);
    }
}
