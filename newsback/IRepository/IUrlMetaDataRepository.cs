using System;
using newsback.Models.UrlModels;

namespace newsback.IRepository;

public interface IUrlMetaDataRepository
{
  Task<bool> AddUrlMetadata(UrlMetadataModel entity);
  Task<List<UrlMetadataModel>> GetAllUrlMetadata();
  Task<UrlMetadataModel?> GetUrlMetadata(string url);
  Task<UrlMetadataModel> UpdateMetaData(UrlMetadataModel entity);
}
