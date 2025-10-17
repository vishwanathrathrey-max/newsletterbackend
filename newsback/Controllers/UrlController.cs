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
        private readonly IUrlMetaDataService _iUrlMetaDataService;

        public UrlController(IHttpClientFactory httpClientFactory, IUrlMetaDataService iUrlMetaDataService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _iUrlMetaDataService = iUrlMetaDataService;
        }


        [HttpPost("GetOpenGrapParameters")]
        public async Task<IActionResult> GetOpenGrapParameters([FromBody] UrlReuestModel request)
        {
            try
            {
                var result = await _iUrlMetaDataService.GetOpenGrapParameters(request.Url);
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
            var result = await _iUrlMetaDataService.GetAllUrlMetadata();
            return Ok(result);
        }

        [HttpPost("AddUrlMetadata")]
        public async Task<IActionResult> AddUrlMetadata([FromBody] UrlMetaDataInsertRequestModel model)
        {
            var result = await _iUrlMetaDataService.AddUrlMetadata(model);
            return Ok(result);
        }
    }
}
