using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Services.Helpers.IHelpers;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Services.Helpers
{
    public class ImageHelper : IImageHelper
    {
        private readonly ISmallFilesService _smallFilesService;

        public ImageHelper(ISmallFilesService smallFilesService)
        {
            this._smallFilesService = smallFilesService;
        }

        public async Task<string?> ResizeAndSaveImage(byte[] arr, int width, int height)
        {
            var file = ResizeImage(arr, width, height);
            if (file == null || !file.Any())
            {
                return null;
            }

            var dto = new SmallFile()
            {
                File = file
            };

            await _smallFilesService.Create(dto);
            return dto?.Id?.ToString() ?? null;
        }

        public byte[] ResizeImage(byte[] arr, int width, int height)
        {
            byte[] arrClonned = new byte[arr.Length];
            arr.CopyTo(arrClonned, 0);
            var format = Image.DetectFormat(arrClonned);

            if (format == null)
            {
                return Array.Empty<byte>();
            }

            using var imgToResize = Image.Load(arrClonned);
            var resizeOptions = new ResizeOptions
            {
                Size = new Size(width, height),
                Sampler = KnownResamplers.Lanczos3,
                Compand = false,
                Mode = ResizeMode.Max
            };

            imgToResize.Mutate(x => x.Resize(resizeOptions));

            using var memoryStr = new MemoryStream();
            imgToResize.SaveAsPng(memoryStr);
            return memoryStr.ToArray();
        }
    }
}