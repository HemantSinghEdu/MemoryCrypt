using System.ComponentModel.DataAnnotations;

namespace MvcClient.Models;

public class Article
{
        public Guid Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        public string? Content { get; set; }
        public int? Views { get; set; }
        public int? UpVotes { get; set; }
}