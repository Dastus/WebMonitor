using HtmlAgilityPack;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Monitor.Application.Interfaces
{
    public interface IHttpRequestService
    {
        Task<HtmlDocument> GetHtmlStructure(string url, TimeSpan timeout);
        Task<HtmlDocument> GetHtmlStructureAsGoogleBotDesktop(string url, TimeSpan timeout);
        Task<HtmlDocument> GetHtmlStructureAsGoogleBotMobile(string url, TimeSpan timeout);
        Task<WebPageLoadResult> GetPageLoadResult(string url, TimeSpan timeout);
        Task<WebPageLoadResult> GetPageLoadResultAsGoogleBot(string url, TimeSpan timeout);
        Task<HttpResponseMessage> PerformGetRequest(string url, TimeSpan timeout, IDictionary<string, string> requestModel = null);
        Task<HttpResponseMessage> PerformPostRequest(string url, TimeSpan timeout, object requestModel = null);
    }
}
