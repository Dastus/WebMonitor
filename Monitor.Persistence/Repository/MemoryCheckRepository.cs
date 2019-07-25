using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Monitor.Application.MonitoringChecks.Models;
using Monitor.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Monitor.Persistence.Repository
{
    public class MemoryCheckRepository : IChecksRepository
    {
        private IMemoryCache _cache;
        const string CACHE_KEY = "checks_list";

        public MemoryCheckRepository(IMemoryCache memoryCache)
        {
            _cache = memoryCache;

            var dict = _cache.Get<Dictionary<CheckTypeEnum, Check>>(CACHE_KEY);
            if (dict == null)
            {
                InitDict();
            }
        }

        public Check GetCheck(CheckTypeEnum checkType)
        {
            var dict = _cache.Get<Dictionary<CheckTypeEnum, Check>>(CACHE_KEY);

            return dict.ContainsKey(checkType) ? dict[checkType] : null;
        }

        public async Task<IEnumerable<Check>> GetCurrentState()
        {
            var dict = _cache.Get<Dictionary<CheckTypeEnum, Check>>(CACHE_KEY);

            return dict.Values;
        }

        public async Task<IEnumerable<Check>> GetCurrentStateForEnvironment(int? environmentId)
        {
            var dict = _cache.Get<Dictionary<CheckTypeEnum, Check>>(CACHE_KEY);

            return (environmentId != null && environmentId.HasValue) ? 
                dict.Values.Where(x => x.EnvironmentId == environmentId.Value) : 
                dict.Values;
        }

        public async Task Save(Check check)
        {
            var dict = _cache.Get<Dictionary<CheckTypeEnum, Check>>(CACHE_KEY);
            if (dict.ContainsKey(check.Type))
            {
                dict[check.Type] = check;
            }
            else
            {
                dict.Add(check.Type, check);
            }            

            _cache.Set(CACHE_KEY, dict);
        }

        private void InitDict()
        {
            //test implementation
            //TODO: implement real DB implementation
            var check1 = new Check
            {
                Id = 1,
                Priority = PrioritiesEnum.Critical,
                Status = StatusesEnum.CRITICAL,
                Service = "Autodoc site availability",
                Description = "Если статус 'CRITICAL', то сайт лежит",
                Type = CheckTypeEnum.HomePageAvailableProd
            };

            var check2 = new Check
            {
                Id = 2,
                Priority = PrioritiesEnum.Medium,
                Status = StatusesEnum.CRITICAL,
                Service = "Meta tags",
                Description = "Если статус 'CRITICAL', то нет метатегов",
                Type = CheckTypeEnum.MetaTagsProd
            };

            var check3 = new Check
            {
                Priority = PrioritiesEnum.Medium,
                Status = StatusesEnum.CRITICAL,
                Service = "Web UI Search",
                Description = "Если статус 'CRITICAL', то не работает поиск",
                Type = CheckTypeEnum.WebUISearchProd
            };

            var dict = new Dictionary<CheckTypeEnum, Check> {
                { CheckTypeEnum.HomePageAvailableProd, check1 },
                { CheckTypeEnum.MetaTagsProd, check2}
            };

            _cache.Set(CACHE_KEY, dict);
        }
    }
}
