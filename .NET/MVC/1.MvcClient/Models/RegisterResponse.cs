
using System.Net;

namespace MvcClient.Models;

public class RegisterResponse
{
    public HttpStatusCode Status { get; set; }

    public string Message { get; set; }

}