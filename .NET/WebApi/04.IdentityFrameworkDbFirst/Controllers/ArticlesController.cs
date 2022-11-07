using IdentityFrameworkDbFirst.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityFrameworkDbFirst.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ArticlesController : ControllerBase
{    
    private readonly ILogger<ArticlesController> _logger;
    private MemoryCryptContext _context;

    public ArticlesController(ILogger<ArticlesController> logger, MemoryCryptContext context)
    {
        _logger = logger;
        _context = context;
    }


    [HttpGet]
    public ActionResult<IEnumerable<Article>> GetArticles()
    {
        return Ok(_context.Articles.ToList());
    }


    [HttpGet("{id}")]
    public ActionResult<Article> GetArticles(string id)
    {
        var guid = new Guid(id);
        var article = _context.Articles.FirstOrDefault(a => a.Id.Equals(guid));
        if(article==null)
        {
            return NotFound();
        }

        return Ok(article);
    }

    [HttpPost]
    public ActionResult<Article> InsertArticle(Article article)
    {
        article.Id = Guid.NewGuid();
        _context.Articles.Add(article);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetArticles), new {id = article.Id}, article);
    }


    [HttpPut("{id}")]
    public ActionResult<Article> UpdateArticle(string id, Article article)
    {
        if(id != article.Id.ToString())
        {
            return BadRequest();
        }

        var guid = new Guid(id);
        var articleToUpdate = _context.Articles.FirstOrDefault(a => a.Id.Equals(guid));

        if(articleToUpdate==null)
        {
            return NotFound();
        }

        articleToUpdate.Author = article.Author;
        articleToUpdate.Content = article.Content;
        articleToUpdate.Title = article.Title;
        articleToUpdate.UpVotes = article.UpVotes;
        articleToUpdate.Views = article.Views;

        _context.Articles.Update(articleToUpdate);
        _context.SaveChanges();

        return NoContent();
    }


    [HttpDelete("{id}")]
    public ActionResult DeleteArticle(string id)
    {
        var guid = new Guid(id);
        var articleToDelete = _context.Articles.FirstOrDefault(a => a.Id.Equals(guid));

        if(articleToDelete == null)
        {
            return NotFound();
        }

        _context.Articles.Remove(articleToDelete);
        _context.SaveChanges();
        return NoContent();
    }

}
