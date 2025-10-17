using System;
using newsback.IRepository;
using newsback.IService;
using newsback.Models.UrlModels;

namespace newsback.Service;

public class UrlMetaDataService : IUrlMetaDataService
{
  private readonly IUrlMetaDataRepository _iUrlMetaDataRepository;
  private readonly IHttpClientFactory _httpClientFactory;

  public UrlMetaDataService(IUrlMetaDataRepository iUrlMetaDataRepository, IHttpClientFactory httpClientFactory)
  {
    _iUrlMetaDataRepository = iUrlMetaDataRepository;
    _httpClientFactory = httpClientFactory;
  }

  public async Task<UrlMetadataModel> AddUrlMetadata(UrlMetaDataInsertRequestModel entity)
  {
    var existing = await _iUrlMetaDataRepository.GetUrlMetadata(entity.Url);
    if (existing != null)
      return existing;

    var metadata = new UrlMetadataModel
    {
      Url = entity.Url,
      Title = entity.Title,
      Description = entity.Description,
      Image = entity.Image,
      RetrievedAt = DateTime.UtcNow
    };
    return await _iUrlMetaDataRepository.AddUrlMetadata(metadata);
  }

  public async Task<List<UrlMetadataModel>> GetAllUrlMetadata()
  {
    return await _iUrlMetaDataRepository.GetAllUrlMetadata();
  }

  public async Task<UrlMetadataModel?> GetUrlMetadata(string url)
  {
    return await _iUrlMetaDataRepository.GetUrlMetadata(url);
  }

  public async Task<UrlMetadataModel> UpdateMetaData(UrlMetadataModel entity)
  {
    return await _iUrlMetaDataRepository.UpdateMetaData(entity);
  }

  public async Task<UrlResponseModel> GetOpenGrapParameters(string url)
  {
    if (!Uri.TryCreate(url, UriKind.Absolute, out var validatedUrl))
      throw new ArgumentException("Invalid URL format.");

    var httpClient = _httpClientFactory.CreateClient("OpenGraphClient");

    var response = await httpClient.GetAsync(validatedUrl);
    if (!response.IsSuccessStatusCode)
      throw new InvalidOperationException("Unable to fetch URL");

    var html = await response.Content.ReadAsStringAsync();
    var doc = new HtmlAgilityPack.HtmlDocument();
    doc.LoadHtml(html);

    string GetMetaContent(string property, string fallback = "")
    {
      var node = doc.DocumentNode.SelectSingleNode(
          $"//meta[@property='{property}'] | //meta[@name='{property}']"
      );
      return string.IsNullOrWhiteSpace(node?.GetAttributeValue("content", "")) ? fallback : node.GetAttributeValue("content", "");
    }

    string title = GetMetaContent("og:title");
    if (string.IsNullOrWhiteSpace(title))
      title = doc.DocumentNode.SelectSingleNode("//title")?.InnerText ?? "Untitled";

    string description = GetMetaContent("og:description");
    if (string.IsNullOrWhiteSpace(description))
    {
      description = GetMetaContent("description");
      if (string.IsNullOrWhiteSpace(description))
      {
        var firstParagraph = doc.DocumentNode.SelectSingleNode("//p")?.InnerText;
        description = !string.IsNullOrWhiteSpace(firstParagraph)
            ? firstParagraph.Substring(0, Math.Min(160, firstParagraph.Length))
            : "No description available.";
      }
    }

    string imageUrl = GetMetaContent("og:image") ?? "";
    if (string.IsNullOrWhiteSpace(imageUrl))
      imageUrl = $"{validatedUrl.Scheme}://{validatedUrl.Host}/favicon.ico";

    if (!string.IsNullOrEmpty(imageUrl) && !imageUrl.StartsWith("http"))
    {
      var baseUri = new Uri(url);
      if (!imageUrl.StartsWith("/")) imageUrl = "/" + imageUrl;
      imageUrl = $"{baseUri.Scheme}://{baseUri.Host}{imageUrl}";
    }

    return new UrlResponseModel
    {
      Title = title,
      Description = description,
      Image = imageUrl
    };
  }

}
