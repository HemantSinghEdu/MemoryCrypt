using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;

namespace MvcClient.Controllers;

public class ArticlesController: Controller
{

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    }
}