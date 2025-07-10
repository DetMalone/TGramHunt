using System.Collections.Generic;
using System.Threading.Tasks;
using TGramHunt.Contract;

namespace TGramHunt.Services.Helpers.IHelpers
{
    public interface ITelegramUserService
    {
        Task<User?> GetAndCreateIfNotExistsAuthTelegramWidget(Dictionary<string, string> query);

        string GetAuthTelegramWidget();
    }
}