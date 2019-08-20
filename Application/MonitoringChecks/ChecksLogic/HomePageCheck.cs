using Monitor.Application.MonitoringChecks.Commands;
using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Net;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Helpers;
using AutoMapper;

namespace Monitor.Application.MonitoringChecks
{
    public class HomePageCheck
    {
        private readonly IHttpRequestService _httpService;

        public HomePageCheck(IHttpRequestService httpService)
        {
            _httpService = httpService;
        }

        public async Task<Check> CheckHomePageLoad(CheckSettings settings)
        {
            var result = new Check { Settings = settings};
            result.State.LastCheckTime = DateTime.Now;

            string address = new EnvironmentHelper().GetEnvironmentUrl(settings.EnvironmentId);

            try
            {
                var loadResult = await _httpService.GetPageLoadResult(address, TimeSpan.FromSeconds(30));

                if (loadResult.ResponseStatus != HttpStatusCode.OK)
                {
                    result.State.Status = StatusesEnum.CRITICAL;
                    result.State.Description = "Сайт недоступен";
                    return result;
                }

                if (loadResult.LoadTime > TimeSpan.FromSeconds(3))
                {
                    result.State.Status = StatusesEnum.WARNING;
                    result.State.Description = "Время загрузки более 3 сек: " + loadResult.LoadTime.Seconds;
                    return result;
                }

                result.State.Status = StatusesEnum.OK;
                result.State.Description = "Проблем не обнаружено";
            }
            catch(Exception ex)
            {
                result.State.Status = StatusesEnum.CRITICAL;
                result.State.Description = "Исключение во время проверки: " + ex.Message;
            }

            return result;
        }
    }
}
