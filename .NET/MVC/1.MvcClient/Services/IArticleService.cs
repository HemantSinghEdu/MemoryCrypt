using MvcClient.Models;

namespace MvcClient.Services;

public interface IArticleService
{
    Task<ArticlesResponse> GetArticlesAsync();
    Task<ArticlesResponse> CreateArticleAsync(Article article);   
}