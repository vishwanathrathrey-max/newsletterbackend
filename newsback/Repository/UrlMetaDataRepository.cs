using System;
using newsback.Data;
using newsback.IRepository;
using newsback.Models.UrlModels;
using Microsoft.EntityFrameworkCore;


namespace newsback.Repository;

public class UrlMetaDataRepository : IUrlMetaDataRepository
{
  private readonly ApplicationDbContext _db;

  public UrlMetaDataRepository(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<UrlMetadataModel> AddUrlMetadata(UrlMetadataModel entity)
  {
    _db.UrlMetadatas.Add(entity);
    await _db.SaveChangesAsync();
    return entity;
  }

  public async Task<List<UrlMetadataModel>> GetAllUrlMetadata()
  {
    return await _db.UrlMetadatas.ToListAsync();
  }

  public async Task<UrlMetadataModel?> GetUrlMetadata(string url)
  {
    return await _db.UrlMetadatas.FirstOrDefaultAsync(x => x.Url == url);
  }

  public async Task<UrlMetadataModel> UpdateMetaData(UrlMetadataModel entity)
  {
    _db.UrlMetadatas.Update(entity);
    await _db.SaveChangesAsync();
    return entity;
  }
}
