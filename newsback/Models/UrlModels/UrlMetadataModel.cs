using System;
using System.ComponentModel.DataAnnotations;

namespace newsback.Models.UrlModels;

public class UrlMetadataModel
{
  [Key]
  public int Id { get; set; }

  [Required]
  public string Url { get; set; } = "";

  public string Title { get; set; } = "";
  public string Description { get; set; } = "";
  public string? Image { get; set; }

  public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
}
