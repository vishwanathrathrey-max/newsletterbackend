using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using newsback.IService;
using newsback.Models.UrlModels;

namespace newsback.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IUrlMetaDataService urlMetaDataService;

        public UrlController(IHttpClientFactory httpClientFactory, IUrlMetaDataService _urlMetaDataService)
        {
            _httpClient = httpClientFactory.CreateClient();
            urlMetaDataService = _urlMetaDataService;
        }


        [HttpPost("GetOpenGrapParameters")]
        public async Task<IActionResult> GetOpenGraphData([FromBody] UrlReuestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await urlMetaDataService.FetchAndSaveOpenGraphData(request.Url);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching OG data: {ex.Message}");
            }
        }

        [HttpGet("GetAllUrlMetadata")]
        public async Task<IActionResult> GetAllUrlMetadata()
        {
            var result = await urlMetaDataService.GetAllUrlMetadata();
            return Ok(result);
        }

    }
}
