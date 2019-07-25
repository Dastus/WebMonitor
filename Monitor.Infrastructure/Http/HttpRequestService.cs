using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Infrastructure.Http
{
    public class HttpRequestService : IHttpRequestService
    {
        public async Task<HtmlDocument> GetHtmlStructure(string url)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(url);
                var stream = await result.Content.ReadAsStreamAsync();

                var doc = new HtmlDocument();
                doc.Load(stream);

                return doc;
            }
        }

        public async Task<WebPageLoadResult> GetPageLoadResult(string url)
        {
            using (var client = new HttpClient())
            {
                var startTime = DateTime.Now;
                var resp = await client.GetAsync(url);
                var endTime = DateTime.Now;

                return new WebPageLoadResult
                {
                    ResponseStatus = resp.StatusCode,
                    LoadTime = (endTime - startTime)
                };
            }
        }
    }
}
