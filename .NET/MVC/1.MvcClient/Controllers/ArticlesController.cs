using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Services;

namespace MvcClient.Controllers;

[Authorize]
public class ArticlesController : Controller
{
    private IArticleService _articleService;
    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
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
        var articlesResponse = await _articleService.GetArticlesAsync();
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
        if (!ModelState.IsValid)
        {
            //return error messages
            return View(article);
        }

        var response = await _articleService.CreateArticleAsync(article);
        if (response.Status == HttpStatusCode.Unauthorized)
        {
            //if response is 401, it means access token has expired
            return RedirectToAction("refresh", "auth", new { returnUrl = "/articles/createArticle" });
        }
        if (response.Status != HttpStatusCode.OK)
        {
            ModelState.AddModelError(string.Empty, "An Error Occurred while processing this request. Please try again in some time.");
        }
        return RedirectToAction("getArticles");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var response = await _articleService.GetArticleAsync(id);
        if (response.Status == HttpStatusCode.Unauthorized)
        {
            //if response is 401, it means access token has expired
            return RedirectToAction("refresh", "auth", new { returnUrl = "/articles/edit" });
        }
        var article = response.Articles.FirstOrDefault();
        return View(article);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Article article)
    {
        var response = await _articleService.UpdateArticleAsync(article);
        if (response.Status == HttpStatusCode.Unauthorized)
        {
            //if response is 401, it means access token has expired
            return RedirectToAction("refresh", "auth", new { returnUrl = "/articles/createArticle" });
        }
         if (response.Status != HttpStatusCode.OK)
        {
            ModelState.AddModelError(string.Empty, "An Error Occurred while processing this request. Please try again in some time.");
        }
        return RedirectToAction("getArticles");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        var response = await _articleService.GetArticleAsync(id);
        if (response.Status == HttpStatusCode.Unauthorized)
        {
            //if response is 401, it means access token has expired
            return RedirectToAction("refresh", "auth", new { returnUrl = "/articles/edit" });
        }
        var article = response.Articles.FirstOrDefault();
        return View(article);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Article article)
    {
        var response = await _articleService.DeleteArticleAsync(article);
        if (response.Status == HttpStatusCode.Unauthorized)
        {
            //if response is 401, it means access token has expired
            return RedirectToAction("refresh", "auth", new { returnUrl = "/articles/delete" });
        }
        if (response.Status != HttpStatusCode.OK)
        {
            ModelState.AddModelError(string.Empty, "An Error Occurred while processing this request. Please try again in some time.");
        }
        return RedirectToAction("getArticles");
    }
}