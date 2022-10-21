using System.Net;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Services;

namespace MvcClient.Controllers;

public class AuthController: Controller
{
    private IApiCallerService _apiService;
    public AuthController(IApiCallerService apiService)
    {
        _apiService = apiService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        if(!ModelState.IsValid)
        {
            //return error messages
            return View(registerRequest);
        }
        else
        {
            //register the user info
            var response = await _apiService.RegisterUser(registerRequest);

            if(response.Status != HttpStatusCode.OK)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(registerRequest);
            }
            // //perform silent login 
            // var loginRequest = new LoginRequest{
            //     Email = registerRequest.Email,
            //     Password = registerRequest.Password
            // };

            // await _apiService.Login(loginRequest);

            //redirect to landing page
            return Ok("Registration Successful");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        if(!ModelState.IsValid)
        {
            //return error messages
            return View(loginRequest);
        }
        else
        {
            //log the user in
            var response = await _apiService.Login(loginRequest);

            if(response.Status != HttpStatusCode.OK)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(loginRequest);
            }

            //Instruct the browser to store the auth tokens in a cookie
            //TODO

            //redirect to landing page
            return RedirectToAction("Index", "Articles");
        }
    }


    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        //log the user out
        LogoutUser();

        //show logout page
        return View();
    }


    private void LogoutUser()
    {
        //log the user out
    }
}