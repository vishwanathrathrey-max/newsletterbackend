using System;
using System.ComponentModel.DataAnnotations;

namespace newsback.Models.UrlModels;

public class UrlReuestModel
{
  [Required(ErrorMessage = "URL is required")]
  [Url(ErrorMessage = "Invalid URL format")]
  public string Url { get; set; }
}
