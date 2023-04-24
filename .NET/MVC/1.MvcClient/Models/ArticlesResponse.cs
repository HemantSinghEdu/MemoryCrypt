using System.Net;

namespace MvcClient.Models;

public class ArticlesResponse
{
    public HttpStatusCode Status { get; set; }
    public string Message { get; set; }

    public IEnumerable<Article> Articles { get; set; }
}