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

        public void InitDict()
        {
            //test implementation
            var schedule = new Dictionary<CheckTypeEnum, CheckSettings>
            {
                { CheckTypeEnum.HomePageAvailableProd ,
                    new CheckSettings
                    {
                        CheckType =CheckTypeEnum.HomePageAvailableProd,
                        Schedule ="* * * * *"
                    }
                }, //every minute

                { CheckTypeEnum.MetaTagsProd ,
                    new CheckSettings
                    {
                        CheckType =CheckTypeEnum.MetaTagsProd,
                        Schedule ="*/3 * * * *"
                    }
                }  //every 3 minutes

                ,{ CheckTypeEnum.WebUISearchProd ,
                    new CheckSettings
                    {
                        CheckType =CheckTypeEnum.WebUISearchProd,
                        Schedule ="*/2 * * * *"
                    }
                },

                { CheckTypeEnum.HomePageAvailableBeta ,
                    new CheckSettings
                    {
                        CheckType =CheckTypeEnum.HomePageAvailableBeta,
                        Schedule ="* * * * *"
                    }
                }, //every minute

                { CheckTypeEnum.MetaTagsBeta ,
                    new CheckSettings
                    {
                        CheckType =CheckTypeEnum.MetaTagsBeta,
                        Schedule ="*/3 * * * *"
                    }
                }  //every 3 minutes

                ,{ CheckTypeEnum.WebUISearchBeta ,
                    new CheckSettings
                    {
                        CheckType =CheckTypeEnum.WebUISearchBeta,
                        Schedule ="*/2 * * * *"
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
