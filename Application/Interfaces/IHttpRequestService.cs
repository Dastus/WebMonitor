﻿using HtmlAgilityPack;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Threading.Tasks;

namespace Monitor.Application.Interfaces
{
    public interface IHttpRequestService
    {
        Task<HtmlDocument> GetHtmlStructure(string url, TimeSpan timeout);
        Task<WebPageLoadResult> GetPageLoadResult(string url, TimeSpan timeout);
    }
}
