using Microsoft.Extensions.Caching.Memory;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Monitor.Persistence.Repository
{
    public class MemoryScheduleRepository : IScheduleRepository
    {
        private IMemoryCache _cache;
        const string CACHE_KEY = "schedule_cache";

        public MemoryScheduleRepository(IMemoryCache memoryCache)
        {
            _cache = memoryCache;

            var dict = _cache.Get<Dictionary<CheckTypeEnum, CheckSettings>>(CACHE_KEY);
            if (dict == null)
            {
                InitDict();
            }
        }

        //Not used now
        public void InitDict()
        {
            //test implementation
            var schedule = new Dictionary<CheckTypeEnum, CheckSettings>
            {
                { CheckTypeEnum.HomePageAvailableProd ,
                    new CheckSettings
                    {
                        Type =CheckTypeEnum.HomePageAvailableProd,
                        NormalSchedule ="* * * * *"
                    }
                }, //every minute

                { CheckTypeEnum.CategoryMetaTagsProd ,
                    new CheckSettings
                    {
                        Type =CheckTypeEnum.CategoryMetaTagsProd,
                        NormalSchedule ="*/3 * * * *"
                    }
                }  //every 3 minutes

                ,{ CheckTypeEnum.WebUISearchProd ,
                    new CheckSettings
                    {
                        Type =CheckTypeEnum.WebUISearchProd,
                        NormalSchedule ="*/2 * * * *"
                    }
                },

                { CheckTypeEnum.HomePageAvailableBeta ,
                    new CheckSettings
                    {
                        Type =CheckTypeEnum.HomePageAvailableBeta,
                        NormalSchedule ="* * * * *"
                    }
                }, //every minute

                { CheckTypeEnum.CategoryMetaTagsBeta ,
                    new CheckSettings
                    {
                        Type =CheckTypeEnum.CategoryMetaTagsBeta,
                        NormalSchedule ="*/3 * * * *"
                    }
                }  //every 3 minutes

                ,{ CheckTypeEnum.WebUISearchBeta ,
                    new CheckSettings
                    {
                        Type =CheckTypeEnum.WebUISearchBeta,
                        NormalSchedule ="*/2 * * * *"
                    }
                }
            };

            _cache.Set(CACHE_KEY, schedule);
        }

        public async Task<Dictionary<CheckTypeEnum, CheckSettings>> GetSchedule()
        {
            return _cache.Get(CACHE_KEY) as Dictionary<CheckTypeEnum, CheckSettings>;
        }
    }
}
