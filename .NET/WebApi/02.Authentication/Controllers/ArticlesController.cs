using _02.Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _02.Authentication.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ArticlesController : ControllerBase
{
    private static List<Article> articles = new List<Article>();
    
    private readonly ILogger<ArticlesController> _logger;

    public ArticlesController(ILogger<ArticlesController> logger)
    {
        _logger = logger;
    }


    [HttpGet]
    public ActionResult<IEnumerable<Article>> GetArticles()
    {
        return Ok(articles);
    }


    [HttpGet("{id}")]
    public ActionResult<Article> GetArticles(string id)
    {
        var article = articles.FirstOrDefault(a => a.Id.Equals(id));
        if(article==null)
        {
            return NotFound();
        }

        return Ok(article);
    }

    [HttpPost]
    public ActionResult<Article> InsertArticle(Article article)
    {
        article.Id = Guid.NewGuid().ToString();
        articles.Add(article);
        return CreatedAtAction(nameof(GetArticles), new {id = article.Id}, article);
    }


    [HttpPut("{id}")]
    public ActionResult<Article> UpdateArticle(string id, Article article)
    {
        if(id != article.Id)
        {
            return BadRequest();
        }

        var articleToUpdate = articles.FirstOrDefault(a => a.Id.Equals(id));

        if(articleToUpdate==null)
        {
            return NotFound();
        }

        articleToUpdate.Author = article.Author;
        articleToUpdate.Content = article.Content;
        articleToUpdate.Title = article.Title;
        articleToUpdate.UpVotes = article.UpVotes;
        articleToUpdate.Views = article.Views;

        return NoContent();
    }


    [HttpDelete("{id}")]
    public ActionResult DeleteArticle(string id)
    {
        var articleToDelete = articles.FirstOrDefault(a => a.Id.Equals(id));

        if(articleToDelete == null)
        {
            return NotFound();
        }

        articles.Remove(articleToDelete);

        return NoContent();
    }

}
