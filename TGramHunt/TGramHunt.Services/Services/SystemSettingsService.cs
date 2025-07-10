using Microsoft.Extensions.Caching.Memory;
using System;
using TGramHunt.Contract;
using TGramHunt.Data.Repositories.IRepositories;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Services.Services
{
    public class SystemSettingsService : ISystemSettingsService
    {
        private static readonly object lockObject =
            new();

        private readonly int liveSpanInMinutes =
            5;

        private readonly string settingsKey = "system_settings_cache_key";

        private readonly IMemoryCache _memoryCache;

        private readonly ISystemSettingsRepository _systemSettingsRepository;

        public SystemSettingsService(ISystemSettingsRepository systemSettingsRepository,
            IMemoryCache memoryCache)
        {
            this._systemSettingsRepository = systemSettingsRepository;
            this._memoryCache = memoryCache;
        }

        public SystemSetting GetSettings()
        {
            var flag = _memoryCache.TryGetValue(settingsKey, out var value);
            if (!flag)
            {
                lock (lockObject)
                {
                    if (_memoryCache.TryGetValue(settingsKey, out value))
                    {
                        return (SystemSetting)value;
                    }

                    var setting = this
                        ._systemSettingsRepository
                        .GetSettings();

                    var option = new MemoryCacheEntryOptions();
                    option.SetSlidingExpiration(TimeSpan.FromMinutes(liveSpanInMinutes));

                    _memoryCache.Set(
                        settingsKey,
                        setting,
                        option);

                    return setting;
                }
            }

            return (SystemSetting)value;
        }
    }
}