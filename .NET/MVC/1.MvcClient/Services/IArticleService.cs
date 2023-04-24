using MvcClient.Models;

namespace MvcClient.Services;

public interface IArticleService
{
    Task<ArticlesResponse> GetArticlesAsync();
    Task<ArticlesResponse> CreateArticleAsync(Article article);   

    Task<ArticlesResponse> GetArticleAsync(string id);
    Task<ArticlesResponse> UpdateArticleAsync(Article article);
    Task<ArticlesResponse> DeleteArticleAsync(Article article);

}