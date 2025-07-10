using System.Threading.Tasks;
using TGramHunt.Contract;

namespace TGramHunt.Services.Helpers.IHelpers
{
    public interface ITgTagHelper
    {
        Task<TagResult>? CheckTag(string tag, bool getInfo);
    }
}
