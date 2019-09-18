using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using Monitor.Application.Interfaces;
using System.Collections.Generic;
using Monitor.Application.MonitoringChecks.Extensions;
using Monitor.Application.MonitoringChecks.Helpers;
using System.Net.Http;

namespace Monitor.Application.MonitoringChecks
{
    public class ProductMetatagsCheck
    {
        private readonly IHttpRequestService _httpService;

        public ProductMetatagsCheck(IHttpRequestService httpService)
        {
            _httpService = httpService;
        }

        public async Task<Check> CheckMetaInfo(CheckSettings settings)
        {
            var result = new Check{ Settings = settings };
            result.State.LastCheckTime = DateTime.Now;
            var requestTimeout = TimeSpan.FromSeconds(30);

            string address = new EnvironmentHelper().GetEnvironmentUrl(settings.EnvironmentId) + "/product/filtr-maslyanyj-dvigatelya-lanos-aveo-lacetti-nubira-nexia-pr-vo-knecht-mahle-id-84669-0-170";

            try
            {
                var warningThreshold = 3;
                var errors = new List<string>();

                var startTime = DateTime.Now;
                var htmlResult = await _httpService.GetHtmlStructureAsGoogleBot(address, requestTimeout);
                var endTime = DateTime.Now;

                var title = htmlResult.GetTitle();
                var expectedTitle = (settings.EnvironmentId == (int)EnvironmentsEnum.Prod) 
                    ? "Фильтр масляный двигателя lanos, aveo, lacetti, nubira, nexia (пр-во knecht-mahle). Масляный фильтр OC90 KNECHT-MAHLE | autodoc.ua" 
                    : "Фильтр масляный lanos, aveo, lacetti, nubira, nexia (пр-во knecht-mahle). Масляный фильтр OC90 KNECHT | autodoc.ua"; 

                if (title != expectedTitle)
                {
                    errors.Add("title incorrect: " + title);
                }

                var robots = htmlResult.GetMetaTagContent("robots");

                if (settings.EnvironmentId == (int)EnvironmentsEnum.Prod && robots != "index, follow")
                {
                    errors.Add("'robots' tag content incorrect: " + robots);
                }

                var keywords = htmlResult.GetMetaTagContent("keywords");
                var expectedKeywords = (settings.EnvironmentId == (int)EnvironmentsEnum.Prod)
                    ? "фильтр масляный двигателя lanos, aveo, lacetti, nubira, nexia (пр-во knecht-mahle), масляный фильтр, OC90, knecht-mahle"
                    : "фильтр масляный lanos, aveo, lacetti, nubira, nexia (пр-во knecht-mahle), масляный фильтр, OC90, knecht";

                if (keywords != expectedKeywords)
                {
                    errors.Add("'keywords' tag content incorrect: " + keywords);
                }

                var description = htmlResult.GetMetaTagContent("description");
                var expectedDescription = (settings.EnvironmentId == (int)EnvironmentsEnum.Prod)
                    ? "Желаете приобрести масляный фильтр OC90 KNECHT-MAHLE? В нашем интернет-магазине вы можете приобрести Фильтр масляный двигателя LANOS, AVEO, LACETTI, NUBIRA, NEXIA (пр-во KNECHT-MAHLE) хорошего качества. На autodoc.ua вы без трудностей сможете подобрать масляный фильтр."
                    : "Искали масляный фильтр OC90 KNECHT? В нашем интернет-магазине вы можете приобрести Фильтр масляный LANOS, AVEO, LACETTI, NUBIRA, NEXIA (пр-во Knecht-Mahle) отличного качества. На сайте autodoc.ua вы быстро сумеете выбрать масляный фильтр.";

                if (description != expectedDescription)
                {
                    errors.Add("'description' tag content incorrect: " + description);
                }

                if (errors.Count() > 0)
                {
                    result.State.Status = StatusesEnum.CRITICAL;
                    result.State.Description = "Обнаружены следующие проблемы: " + string.Join(",", errors);
                    return result;
                }

                var execTime = endTime - startTime;
                if (execTime > TimeSpan.FromSeconds(warningThreshold))
                {
                    result.State.Status = StatusesEnum.WARNING;
                    result.State.Description = string.Format("Время ответа больше порога {0} сек: {1:0.00}", warningThreshold, execTime.TotalSeconds);
                    return result;
                }

                result.State.Status = StatusesEnum.OK;
                result.State.Description = "Проблем не обнаружено";
            }
            catch (TaskCanceledException)
            {
                result.State.Status = StatusesEnum.CRITICAL;
                result.State.Description = string.Format("Превышен интервал выполнения запроса: {0} сек", requestTimeout.Seconds);
            }
            catch (HttpRequestException ex)
            {
                result.State.Status = StatusesEnum.CRITICAL;
                result.State.Description = "Ошибка http-запроса: " + ex.Message;
            }
            catch
            {
                result.State.Status = StatusesEnum.CRITICAL;
                result.State.Description = "Ошибка при обработке HTML";
            }

            return result;
        }
    }
}
