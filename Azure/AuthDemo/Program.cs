using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace AuthDemo
{
    public class Program
    {
        private const string _clientId = "d95843c6-b957-406c-9aec-7d86b473a6f5";
        private const string _tenantId = "b5ee3756-7ac5-4f4b-a8f3-36fa01d703dc";

        public static async Task Main(string[] args)
        {
            var app = PublicClientApplicationBuilder
                    .Create(_clientId)
                    .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
                    .WithRedirectUri("http://localhost")
                    .Build();

            string[] scopes = { "user.read" };

            AuthenticationResult result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            Console.WriteLine($"Token:\t{result.AccessToken}");
        }
    }
}