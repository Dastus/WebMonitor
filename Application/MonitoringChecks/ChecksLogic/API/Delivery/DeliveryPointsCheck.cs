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
    public class DeliveryPointsCheck
    {
        private TimeSpan timeOut = TimeSpan.FromSeconds(30);
        private readonly IHttpRequestService _httpService;

        public DeliveryPointsCheck(IHttpRequestService httpService)
        {
            _httpService = httpService;
        }

        public async Task<Check> CheckApi(CheckSettings settings, DeliveryPointsRequest requestModel)
        {
            var result = new Check { Settings = settings };
            result.State.LastCheckTime = DateTime.Now;

            return await PerformCheck(settings.EnvironmentId, result, requestModel);
        }

        private async Task<Check> PerformCheck(int environmentId, Check check, DeliveryPointsRequest requestModel)
        {
            check.State.Status = StatusesEnum.CRITICAL;

            var address = new EnvironmentHelper().GetApiUrl(environmentId);
            var query = $"{address}/api/delivery/points";

            var response = _httpService.PerformPostRequest(query, timeOut, requestModel);

            var result = response.Result;
            if (!result.IsSuccessStatusCode)
            {
                check.State.Description = "Полученный статус-код для точек выдачи: " + result.StatusCode;
                return check;
            }

            try
            {
                var responseContent = await result.Content.ReadAsStringAsync();
                var settlementsResponseModel = JsonConvert.DeserializeObject<ApiResponse<List<DeliveryPointModel>>>(responseContent);

                if (!settlementsResponseModel.Success)
                {
                    var errorsStr = string.Join(".", settlementsResponseModel.Errors);
                    check.State.Description = "Ошибка получения точек выдачи: " + errorsStr;
                    return check;
                }

                if (settlementsResponseModel?.Data?.Count == 0)
                {
                    check.State.Description = "Получен пустой список точек выдачи";
                    return check;
                }
            }
            catch (Exception e)
            {
                check.State.Description = "Исключение во время получения точек выдачи: " + e.Message;
            }

            check.State.Status = StatusesEnum.OK;
            check.State.Description = "Проблем не обнаружено";

            return check;
        }

    }

}
