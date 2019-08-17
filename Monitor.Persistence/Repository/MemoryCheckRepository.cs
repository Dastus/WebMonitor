using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Monitor.Application.MonitoringChecks.Models;
using Monitor.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;
using Monitor.Application.MonitoringChecks.Commands;

namespace Monitor.Persistence.Repository
{
    public class MemoryCheckRepository : IChecksRepository
    {
        private IMemoryCache _cache;
        const string CACHE_KEY = "checks_list";
        private IMapper _mapper;

        public MemoryCheckRepository(IMemoryCache memoryCache, IMapper mapper)
        {
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cache.Set(CACHE_KEY, new Dictionary<CheckTypeEnum, Check>());
        }
        
        public Check GetCheck(CheckTypeEnum checkType)
        {
            var dict = _cache.Get<Dictionary<CheckTypeEnum, Check>>(CACHE_KEY);

            return dict.ContainsKey(checkType) ? dict[checkType] : null;
        }

        public async Task<IEnumerable<CheckWebModel>> GetCurrentState()
        {
            var dict = _cache.Get<Dictionary<CheckTypeEnum, Check>>(CACHE_KEY);
            var result = _mapper.Map<IEnumerable<Check>, IEnumerable<CheckWebModel>>(dict.Values);

            return result;
        }

        public async Task<IEnumerable<CheckWebModel>> GetCurrentStateForEnvironment(int? environmentId)
        {
            var dict = _cache.Get<Dictionary<CheckTypeEnum, Check>>(CACHE_KEY);

            var checks = (environmentId != null && environmentId.HasValue) ? 
                dict.Values.Where(x => x.Settings.EnvironmentId == environmentId.Value) : 
                dict.Values;
            var result = _mapper.Map<IEnumerable<Check>, IEnumerable<CheckWebModel>>(checks);

            return result;
        }

        public async Task Save(Check check)
        {
            var dict = _cache.Get<Dictionary<CheckTypeEnum, Check>>(CACHE_KEY);
            if (dict.ContainsKey(check.Settings.Type))
            {
                dict[check.Settings.Type] = check;
            }
            else
            {
                dict.Add(check.Settings.Type, check);
            }            

            _cache.Set(CACHE_KEY, dict);
        }
    }
}
