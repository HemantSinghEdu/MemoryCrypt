using System;
using System.Collections.Generic;

namespace IdentityFrameworkDbFirst.Models
{
    public partial class Article
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Content { get; set; }
        public int? Views { get; set; }
        public int? UpVotes { get; set; }
    }
}
