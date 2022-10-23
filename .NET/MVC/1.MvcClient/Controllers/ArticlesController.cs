using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Services;

namespace MvcClient.Controllers;

[Authorize]
public class ArticlesController : Controller
{
    private IApiCallerService _apiService;
    public ArticlesController(IApiCallerService apiService)
    {
        _apiService = apiService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetArticles()
    {
        var articlesResponse = await _apiService.RequestArticlesAsync();
        if (articlesResponse.Status == HttpStatusCode.Unauthorized)
        {
            //if response is 401, it means access token has expired
            return RedirectToAction("refresh", "auth", new { returnUrl = "/articles/getArticles" });
        }
        if (articlesResponse.Status != HttpStatusCode.OK)
        {
            ModelState.AddModelError(string.Empty, "An Error Occurred while processing this request. Please try again in some time.");
        }
        return View(articlesResponse.Articles);
    }

    [HttpGet]
    public async Task<IActionResult> CreateArticle()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateArticle(Article article)
    {
        throw new NotImplementedException();
    }
}