using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.ChecksLogic.API.Delivery.Models;
using Monitor.Application.MonitoringChecks.Helpers;
using Monitor.Application.MonitoringChecks.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.ChecksLogic
{
    public class AreasCheck
    {
        private TimeSpan timeOut = TimeSpan.FromSeconds(30);
        private readonly IHttpRequestService _httpService;

        public AreasCheck(IHttpRequestService httpService)
        {
            _httpService = httpService;
        }

        public async Task<Check> CheckApi(CheckSettings settings)
        {
            var result = new Check { Settings = settings };
            result.State.LastCheckTime = DateTime.Now;

            return await PerformCheck(settings.EnvironmentId, result);
        }

        private async Task<Check> PerformCheck(int environmentId, Check check)
        {
            check.State.Status = StatusesEnum.CRITICAL;

            var address = new EnvironmentHelper().GetApiUrl(environmentId);
            var query = $"{address}/api/delivery/areas";

            var response = _httpService.PerformGetRequest(query, timeOut);

            var result = response.Result;
            if (!result.IsSuccessStatusCode)
            {
                check.State.Description = "Полученный статус-код для Областей: " + result.StatusCode;
                return check;
            }

            try
            {
                var responseContent = await result.Content.ReadAsStringAsync();
                var areasResponseModel = JsonConvert.DeserializeObject<ApiResponse<List<Area>>>(responseContent);

                if (!areasResponseModel.Success)
                {
                    var errorsStr = string.Join(".", areasResponseModel.Errors);
                    check.State.Description = "Ошибка получения областей: " + errorsStr;
                    return check;
                }

                if (areasResponseModel?.Data?.Count == 0)
                {                    
                    check.State.Description = "Получен пустой список областей";
                    return check;
                }
            }
            catch(Exception e)
            {
                check.State.Description = "Исключение во время получения областей: " + e.Message;
            }

            check.State.Status = StatusesEnum.OK;
            check.State.Description = "Проблем не обнаружено";

            return check;
        }

    }

}
