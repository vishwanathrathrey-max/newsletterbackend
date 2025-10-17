using System;

namespace newsback.Models.UrlModels;

public class UrlMetaDataInsertRequestModel : UrlResponseModel
{
  public required string Url { get; set; }
}
