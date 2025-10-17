using System;
using newsback.Models.UrlModels;

namespace newsback.IService;

public interface IUrlMetaDataService
{
  Task<bool> AddUrlMetadata(UrlMetaDataInsertRequestModel entity);
  Task<List<UrlMetadataModel>> GetAllUrlMetadata();
  Task<UrlMetadataModel?> GetUrlMetadata(string url);
  Task<UrlMetadataModel> UpdateMetaData(UrlMetadataModel entity);
  Task<UrlResponseModel> GetOpenGrapParameters(string url);
}
