using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Text;
using WebApiEntityFramework.Data;

namespace WebApiEntityFramework.Tests
{
    public class EmployeeControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public EmployeeControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_ReturnsValues()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/Employee");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetById_ReturnsValues()
        {
            var client = _factory.CreateClient();
            var id = SeedData.InitialEmployees.First().EmployeeId;
            var response = await client.GetAsync($"/Employee/{id}");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetById_WithInvalidValue_ReturnsNotFound()
        {
            var client = _factory.CreateClient();
            var id = SeedData.InitialEmployees.First().EmployeeId;
            var response = await client.GetAsync($"/Employee/1234");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Fact]
        public async Task GetByName_ReturnsValues()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/Employee/search/John");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }


        [Fact]
        public async Task Post_WithValidValue_ReturnsOk()
        {
            var client = _factory.CreateClient();

            var content = new StringContent("{ \"firstName\": \"John\", \"lastName\": \"Walsh\", \"age\": 30, \"emailAddress\": \"john.doe@example.com\" }", Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/Employee", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Post_WithDuplicateValue_ReturnsConflict()
        {
            var client = _factory.CreateClient();

            var content = new StringContent("{ \"firstName\": \"John\", \"lastName\": \"Doe\", \"age\": 30, \"emailAddress\": \"john.doe@example.com\" }", Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/Employee", content);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Post_WithEmptyValue_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();

            var content = new StringContent("\"\"", Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/Employee", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Put_WithValidValue_ReturnsNoContent()
        {
            var client = _factory.CreateClient();
            var id = SeedData.InitialEmployees.First().EmployeeId;
            var content = new StringContent("{ \"firstName\": \"John\", \"lastName\": \"Cooper\", \"age\": 30, \"emailAddress\": \"john.doe@example.com\" }", Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/Employee/{id}", content);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Put_WithEmptyValue_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();
            var id = SeedData.InitialEmployees.First().EmployeeId;
            var content = new StringContent("\"\"", Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/Employee/{id}", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Put_WithDuplicateValue_ReturnsConflict()
        {
            var client = _factory.CreateClient();
            var id = SeedData.InitialEmployees.Last().EmployeeId;
            var content = new StringContent("{ \"firstName\": \"John\", \"lastName\": \"Doe\", \"age\": 30, \"emailAddress\": \"john.doe@example.com\" }", Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/Employee/{id}", content);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Put_WithNoChange_ReturnsNoContent()
        {
            var client = _factory.CreateClient();
            var id = SeedData.InitialEmployees.First().EmployeeId;
            var content = new StringContent("{ \"firstName\": \"John\", \"lastName\": \"Doe\", \"age\": 30, \"emailAddress\": \"john.doe@example.com\" }", Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/Employee/{id}", content);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

    }
}