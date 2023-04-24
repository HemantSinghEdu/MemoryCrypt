using System.Net;
using MvcClient.Models;

namespace MvcClient.Services;

public class ArticleService : IArticleService
{
    private IConfiguration _config;
    private IAuthService _authService;
    private IApiCallerService _apiService;
    private string _url;
    public ArticleService(IConfiguration config, IApiCallerService apiService, IAuthService authService)
    {
        _config = config;
        _apiService = apiService;
        _authService = authService;
        _url = _config["apiService:articlesUrl"];
    }

    public async Task<ArticlesResponse> GetArticlesAsync()
    {

        var token = _authService.GetSavedClaims().AuthToken;

        var httpResponse = await _apiService.MakeHttpCallAsync(
            httpMethod: HttpMethod.Get,
            url: _url,
            authScheme: "bearer",
            authToken: token
        );
        ArticlesResponse response = new ArticlesResponse();
        if (httpResponse.StatusCode == HttpStatusCode.OK)
        {
            var articles = await httpResponse.Content.ReadFromJsonAsync<List<Article>>();
            response.Status = httpResponse.StatusCode;
            response.Articles = articles;
        }
        else
        {
            response.Status = httpResponse.StatusCode;
            response.Message = await httpResponse.Content.ReadAsStringAsync();
        }
        return response;
    }

    public async Task<ArticlesResponse> GetArticleAsync(string id)
    {
        var getUrl = $"{_url}/{id}";
        var token = _authService.GetSavedClaims().AuthToken;

        var httpResponse = await _apiService.MakeHttpCallAsync(
            httpMethod: HttpMethod.Get,
            url: getUrl,
            authScheme: "bearer",
            authToken: token
        );
        ArticlesResponse response = new ArticlesResponse();
        if (httpResponse.StatusCode == HttpStatusCode.OK)
        {
            var article = await httpResponse.Content.ReadFromJsonAsync<Article>();
            response.Status = httpResponse.StatusCode;
            response.Articles = new List<Article> { article };
        }
        else
        {
            response.Status = httpResponse.StatusCode;
            response.Message = await httpResponse.Content.ReadAsStringAsync();
        }
        return response;
    }

    public async Task<ArticlesResponse> CreateArticleAsync(Article article)
    {
        var token = _authService.GetSavedClaims().AuthToken;

        var httpResponse = await _apiService.MakeHttpCallAsync(
            httpMethod: HttpMethod.Post,
            url: _url,
            bodyContent: article,
            authScheme: "bearer",
            authToken: token
        );
        ArticlesResponse response = new ArticlesResponse();
        if (httpResponse.StatusCode == HttpStatusCode.OK)
        {
            var createdArticle = await httpResponse.Content.ReadFromJsonAsync<Article>();
            response.Status = httpResponse.StatusCode;
            response.Articles = new List<Article> { createdArticle };
        }
        else
        {
            response.Status = httpResponse.StatusCode;
            response.Message = await httpResponse.Content.ReadAsStringAsync();
        }
        return response;
    }

    public async Task<ArticlesResponse> UpdateArticleAsync(Article article)
    {
        var token = _authService.GetSavedClaims().AuthToken;

        var putUrl = $"{_url}/{article.Id.ToString()}";
        var httpResponse = await _apiService.MakeHttpCallAsync(
            httpMethod: HttpMethod.Put,
            url: putUrl,
            bodyContent: article,
            authScheme: "bearer",
            authToken: token
        );
        ArticlesResponse response = new ArticlesResponse();
        if (httpResponse.StatusCode == HttpStatusCode.OK)
        {
            var createdArticle = await httpResponse.Content.ReadFromJsonAsync<Article>();
            response.Status = httpResponse.StatusCode;
            response.Articles = new List<Article> { createdArticle };
        }
        else
        {
            response.Status = httpResponse.StatusCode;
            response.Message = await httpResponse.Content.ReadAsStringAsync();
        }
        return response;
    }

    public async Task<ArticlesResponse> DeleteArticleAsync(Article article)
    {
        var token = _authService.GetSavedClaims().AuthToken;

        var deleteUrl = $"{_url}/{article.Id.ToString()}";
        var httpResponse = await _apiService.MakeHttpCallAsync(
            httpMethod: HttpMethod.Delete,
            url: deleteUrl,
            authScheme: "bearer",
            authToken: token
        );
        ArticlesResponse response = new ArticlesResponse();
        if (httpResponse.StatusCode == HttpStatusCode.OK)
        {
            response.Status = httpResponse.StatusCode;
            response.Articles = new List<Article> { article };
        }
        else
        {
            response.Status = httpResponse.StatusCode;
            response.Message = await httpResponse.Content.ReadAsStringAsync();
        }
        return response;
    }
}