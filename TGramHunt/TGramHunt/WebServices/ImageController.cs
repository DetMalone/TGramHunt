using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Threading.Tasks;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.WebServices
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ISmallFilesService _smallFilesService;
        public ImageController(ISmallFilesService gridFSBucketService)
        {
            this._smallFilesService = gridFSBucketService;
        }

        // GET api/<ImageController>/62f744079d2f293aa4bf4bcd
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            var respHeaders = Response.Headers;
            respHeaders.Add(HeaderNames.CacheControl, "public,max-age=31556952");
            respHeaders.Add(HeaderNames.Expires, new[] { DateTime.UtcNow.AddYears(1).ToString("R") });

            var dateR = new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToString("R");
            respHeaders.Add(HeaderNames.LastModified, dateR);

            var ifModifiedSince = Request.Headers.IfModifiedSince;
            if (ifModifiedSince.Count > 0)
            {
                var first = ifModifiedSince.FirstOrDefault();
                if (first != null && first.ToString() == dateR)
                {
                    Response.StatusCode = StatusCodes.Status304NotModified;
                    return Content(string.Empty);
                }
            }

            var dto = await this._smallFilesService.Get(id);

            if (dto == null)
            {
                return new NotFoundResult();
            }

            return new FileContentResult(dto.File, "image/gif");
        }
    }
}