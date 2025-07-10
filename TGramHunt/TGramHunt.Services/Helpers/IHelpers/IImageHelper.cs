using System.Threading.Tasks;

namespace TGramHunt.Services.Helpers.IHelpers
{
    public interface IImageHelper
    {
        Task<string?> ResizeAndSaveImage(byte[] arr, int width, int height);
        byte[] ResizeImage(byte[] arr, int width, int height);
    }
}