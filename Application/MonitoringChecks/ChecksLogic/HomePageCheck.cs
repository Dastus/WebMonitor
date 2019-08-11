using Monitor.Application.MonitoringChecks.Commands;
using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Net;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Helpers;

namespace Monitor.Application.MonitoringChecks
{
    public class HomePageCheck
    {
        private readonly IHttpRequestService _httpService;

        public HomePageCheck(IHttpRequestService httpService)
        {
            _httpService = httpService;
        }

        public async Task<Check> CheckHomePageLoad(EnvironmentsEnum environment)
        {
            var environmentId = (int)environment;
            var result = new Check
            {
                Priority = PrioritiesEnum.Critical,
                Service = "Autodoc site availability",
                Type = (environment == EnvironmentsEnum.Prod) ? 
                    CheckTypeEnum.HomePageAvailableProd : CheckTypeEnum.HomePageAvailableBeta,
                LastCheckTime = DateTime.Now,
                EnvironmentId = environmentId
            };

            string address = new EnvironmentHelper().GetEnvironmentUrl(environmentId);

            try
            {
                var loadResult = await _httpService.GetPageLoadResult(address, TimeSpan.FromSeconds(60));

                if (loadResult.ResponseStatus != HttpStatusCode.OK)
                {
                    result.Status = StatusesEnum.CRITICAL;
                    result.Description = "Сайт недоступен";
                    return result;
                }

                if (loadResult.LoadTime > TimeSpan.FromSeconds(3))
                {
                    result.Status = StatusesEnum.WARNING;
                    result.Description = "Время загрузки более 3 сек";
                    return result;
                }

                result.Status = StatusesEnum.OK;
                result.Description = "Главная страница в порядке";
            }
            catch
            {
                result.Status = StatusesEnum.CRITICAL;
                result.Description = "Сайт недоступен";
            }

            return result;
        }
    }
}
