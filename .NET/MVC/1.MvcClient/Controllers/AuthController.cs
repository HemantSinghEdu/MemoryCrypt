using System.Net;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Services;

namespace MvcClient.Controllers;

public class AuthController : Controller
{
    private IApiCallerService _apiService;
    private IAuthService _authService;
    public AuthController(IApiCallerService apiService, IAuthService authService)
    {
        _apiService = apiService;
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        if (!ModelState.IsValid)
        {
            //return error messages
            return View(registerRequest);
        }
        else
        {
            //register the user info
            var response = await _apiService.RequestRegisterAsync(registerRequest);

            if (response.Status != HttpStatusCode.OK)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(registerRequest);
            }

            //silent login
            var loginRequest = new LoginRequest { Email = registerRequest.Email, Password = registerRequest.Password };
            return await Login(loginRequest);
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        if (!ModelState.IsValid)
        {
            //return error messages
            return View(loginRequest);
        }
        else
        {
            //log the user in
            var response = await _apiService.RequestLoginAsync(loginRequest);

            if (response.Status == HttpStatusCode.OK)
            {
                //use identity to create cookie
                await _authService.LoginAsync(response.Email, response.AccessToken, response.RefreshToken);
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(loginRequest);
            }
            return RedirectTo();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Refresh(string returnUrl)
    {
        var response = await _apiService.RequestRefreshAsync();
        if (response.Status == HttpStatusCode.OK)
        {
             await _authService.LoginAsync(response.Email, response.AccessToken, response.RefreshToken);
            return RedirectTo(returnUrl);
        }
        else
        {
            return RedirectToAction("login");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        //log the user out
        await _authService.LogoutAsync();

        //take to home page
        return RedirectTo();
    }

    private IActionResult RedirectTo(string returnUrl = null)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            //default redirect
            return RedirectToAction("Index", "Articles");
        }
    }
}