using System;
using newsback.IRepository;
using newsback.IService;
using newsback.Models.UrlModels;

namespace newsback.Service;

public class UrlMetaDataService : IUrlMetaDataService
{
  private readonly IUrlMetaDataRepository urlRepository;
  private readonly HttpClient httpClient;

  public UrlMetaDataService(IUrlMetaDataRepository repository, HttpClient _httpClient)
  {
    urlRepository = repository;
    httpClient = _httpClient;
  }

  public async Task<UrlMetadataModel> AddUrlMetadata(UrlMetadataModel entity)
  {
    return await urlRepository.AddUrlMetadata(entity);
  }

  public Task<List<UrlMetadataModel>> GetAllUrlMetadata()
  {
    return urlRepository.GetAllUrlMetadata();
  }

  public Task<UrlMetadataModel?> GetUrlMetadata(string url)
  {
    return urlRepository.GetUrlMetadata(url);
  }

  public Task<UrlMetadataModel> UpdateMetaData(UrlMetadataModel entity)
  {
    return urlRepository.UpdateMetaData(entity);
  }

  public async Task<UrlResponseModel> FetchAndSaveOpenGraphData(string url)
  {
    if (!Uri.TryCreate(url, UriKind.Absolute, out var validatedUrl))
      throw new ArgumentException("Invalid URL format.");

    // Fetch HTML
    var response = await httpClient.GetAsync(validatedUrl);
    if (!response.IsSuccessStatusCode)
      throw new InvalidOperationException("Unable to fetch URL");

    var html = await response.Content.ReadAsStringAsync();
    var doc = new HtmlAgilityPack.HtmlDocument();
    doc.LoadHtml(html);

    // Helper function
    string GetMetaContent(string property)
    {
      var node = doc.DocumentNode.SelectSingleNode(
          $"//meta[@property='{property}'] | //meta[@name='{property}']"
      );
      return node?.GetAttributeValue("content", "") ?? "";
    }

    string imageUrl = GetMetaContent("og:image") ?? "";
    if (!string.IsNullOrEmpty(imageUrl) && !imageUrl.StartsWith("http"))
    {
      var baseUri = new Uri(url);
      if (!imageUrl.StartsWith("/")) imageUrl = "/" + imageUrl;
      imageUrl = $"{baseUri.Scheme}://{baseUri.Host}{imageUrl}";
    }

    // Check existing
    var existing = await GetUrlMetadata(validatedUrl.ToString());
    UrlMetadataModel entity;

    if (existing != null)
    {
      existing.Title = GetMetaContent("og:title") ?? "";
      existing.Description = GetMetaContent("og:description") ?? "";
      existing.Image = imageUrl;
      existing.RetrievedAt = DateTime.UtcNow;

      entity = await UpdateMetaData(existing);
    }
    else
    {
      entity = new UrlMetadataModel
      {
        Url = validatedUrl.ToString(),
        Title = GetMetaContent("og:title") ?? "",
        Description = GetMetaContent("og:description") ?? "",
        Image = imageUrl,
        RetrievedAt = DateTime.UtcNow
      };
      entity = await AddUrlMetadata(entity);
    }

    return new UrlResponseModel
    {
      Title = entity.Title,
      Description = entity.Description,
      Image = entity.Image
    };
  }
}
