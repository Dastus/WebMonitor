using Monitor.Application.MonitoringChecks.Helpers;
using Monitor.Application.MonitoringChecks.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.ChecksLogic
{
    public class ApiSearchCheck
    {
        private TimeSpan timeOut = TimeSpan.FromSeconds(30);

        public async Task<Check> CheckApiSearch(CheckSettings settings)
        {
            var result = new Check { Settings = settings };
            result.State.LastCheckTime = DateTime.Now;

            return await PerformSearchCheck(settings.EnvironmentId, result);
        }
        
        private async Task<Check> PerformSearchCheck(int environmentId, Check check)
        {            
            check.State.Status = StatusesEnum.CRITICAL;
            SearchParameters model = new SearchParameters
            {
                SearchPhrase = "oc90"
            };
            AutodocSearchResult productResponse = null;

            var address = new EnvironmentHelper().GetApiUrl(environmentId);
            var query = $"{address}/api/search/filtered";    
            var jsonSearchModel = JsonConvert.SerializeObject(model);
            var requestModel = new SearchRequestModel { SearchParametersJson = jsonSearchModel };

            var content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");

            using (var client = new HttpClient { Timeout = timeOut })
            {
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

                var response = await client.PostAsync(query, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    check.State.Description = "Полученный статус-код: " + response.StatusCode;
                    return check;
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                try
                {
                    productResponse = JsonConvert.DeserializeObject<AutodocSearchResult>(responseContent);
                    if (productResponse == null)
                    {
                        check.State.Description = "С сервера получена неправильная поисковая модель";
                        return check;
                    }
                }
                catch
                {
                    check.State.Description = "Ошибка при парсинге поисковой модели";
                    return check;
                }

                if (productResponse.TotalCount <= 0)
                {
                    check.State.Description = "поле 'Total' в ответе: " + productResponse.TotalCount;
                    return check;
                }

                if (productResponse.Products.Count == 0)
                {
                    check.State.Description = "Ответ не содержит списка товаров";
                    return check;
                }
            }

            check.State.Status = StatusesEnum.OK;
            check.State.Description = "Проблем не обнаружено";
            return check;
        }
        
    }

    #region Search Models
    class SearchRequestModel
    {
        public SearchTypesEnum SearchType { get; set; }
        public string CategoryUrl { get; set; }
        public string SearchParametersJson { get; set; }
    }

    class SearchParameters
    {
        public string SearchPhrase { get; set; } = "";
        public int Rest { get; set; } = 0;
        public int FormFactor { get; set; } = 2;
        public int[] CarTypes { get; set; } = new int[0];//marks 
        public int[] CarModels { get; set; } = new int[0];//models
        public int[] CarModifs { get; set; } = new int[0];//types
        public string[] ProductLines { get; set; } = new string[0];
        public int[] CartManufacturers { get; set; } = new int[0];
        public int From { get; set; } = 0;
        public int Count { get; set; } = 20;
        public string SortField { get; set; } = "";
        public int SortOrder { get; set; } = 0;
        public int[] TreeParts { get; set; } = new int[0];
        public string[] Manufacturers { get; set; } = new string[0];
        public string SelectedCategory { get; set; } = "";
        public string Keyword { get; set; } = "";
        public long ArtId { get; set; } = 0;
    }
    public class AutodocSearchResult
    {
        public List<ClientProduct> Products { get; set; }
        public long TotalCount { get; set; }
    }
    public enum SearchTypesEnum
    {
        Simple = 0,
        Accum = 1,
        Oil = 2,
        TechnicalFluids = 3,
        Tires = 4,
        Optic = 5,
        Lamp = 6
    }

    public enum SearchStatus
    {
        OK = 0,
        More,       // Search result more xxx items (max)
        ESError,	// Error in ElasticSearch
        Empty       // Empty result
    }

    public interface IProduct
    {
        long Id { get; set; }
        string TransliteratedTitle { get; set; }
    }

    public class ClientProduct : IProduct
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public bool InStock { get; set; }
        public string LookupNumber { get; set; }
        public string DisplayDescription { get; set; }
        public string DisplayDescriptionUkr { get; set; }
        public string HardPackingRate { get; set; }
        public string CardId { get; set; }
        public string BrandId { get; set; }
        public long GroupId { get; set; }
        public string Brand { get; set; }
        public string TransliteratedTitle { get; set; }
        public int Is3D { get; set; }
        public int AnalogsCount { get; set; }
        public string Info { get; set; }
        public string ProductLine { get; set; }
        public string Ref_Key { get; set; }
        public string AutodocInfo { get; set; }
        public string AutodocInfoUkr { get; set; }
        public string AutodocInfoRus { get; set; }
        public string Image { get; set; }
        public string Card { get; set; }
    }

    #endregion
}
