using System.Net;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Services;

namespace MvcClient.Controllers;

public class AuthController : Controller
{
    private IAuthService _authService;
    public AuthController(IAuthService authService)
    {
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
            var response = await _authService.RegisterAsync(registerRequest);

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
            //provide login creds to web api and get tokens in response
            var response = await _authService.LoginAsync(loginRequest);

            if (response.Status == HttpStatusCode.OK)
            {
                //log the user in by generating a cookie 
                await _authService.LoginWithCookieAsync(response.Email, response.AccessToken, response.RefreshToken);
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
        var response = await _authService.RefreshTokenAsync();
        if (response.Status == HttpStatusCode.OK)
        {
            await _authService.LoginWithCookieAsync(response.Email, response.AccessToken, response.RefreshToken);
            return RedirectTo(returnUrl);
        }
        else
        {
            //if refresh request failed, redirect to login page
            return RedirectToAction("login");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        //remove stored tokens from database by calling web api
        await _authService.RevokeTokenAsync();
        
        //log the user out by removing cookie from mvc app
        await _authService.LogoutAsync();
        
        //redirect to home page once logged out
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