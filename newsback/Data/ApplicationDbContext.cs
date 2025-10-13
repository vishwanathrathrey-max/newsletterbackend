using Microsoft.EntityFrameworkCore;
using newsback.Models.UrlModels;

namespace newsback.Data
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UrlMetadataModel> UrlMetadatas { get; set; }
  }
}