using System.Net.Http;
using System.Threading.Tasks;
using TGramHunt.Contract;

namespace TGramHunt.Services.Helpers.IHelpers
{
    public interface ITgTagHelperExtensions
    {
        TagResult checkIfEmpty(string url, bool getInfo);

        Task<TagResult> checkSticker(string tag, HttpClient httpClient, string apiUrl, bool getInfo);

    }
}
