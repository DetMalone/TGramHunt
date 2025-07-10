using TGramHunt.Contract;

namespace TGramHunt.Data.Repositories.IRepositories
{
    public interface ISystemSettingsRepository
    {
        SystemSetting GetSettings();
    }
}