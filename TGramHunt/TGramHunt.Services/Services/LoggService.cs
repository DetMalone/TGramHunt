using System;
using System.Threading.Tasks;
using TGramHunt.Contract.Logging;
using TGramHunt.Data.Repositories.IRepositories;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Services.Services
{
    public class LoggService : ILoggService
    {
        private readonly ILoggRepository _loggRepository;
        private readonly ISystemSettingsService _systemSettingsService;

        public LoggService(ILoggRepository loggRepository,
            ISystemSettingsService systemSettingsService)
        {
            this._loggRepository = loggRepository;
            this._systemSettingsService = systemSettingsService;
        }

        public async Task Log(string message, string userName)
        {
            if (!this._systemSettingsService.GetSettings().IsLoggingEnabled ||
                string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            await _loggRepository.Log(new LoggingDto()
            {
                Type = LoggingType.Info,
                Message = message,
                UserName = userName
            });
        }

        public async Task Log(Exception ex, string userName)
        {
            if (!this._systemSettingsService.GetSettings().IsLoggingEnabled ||
                ex == null)
            {
                return;
            }

            await _loggRepository.Log(new LoggingDto()
            {
                Type = LoggingType.Error,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                ExceptionType = ex.GetType().ToString(),
                UserName = userName
            });
        }
    }
}