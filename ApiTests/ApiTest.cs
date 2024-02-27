
using Microsoft.AspNetCore.Mvc.Testing;
using TeamServer;

namespace ApiTests
{
    public abstract class ApiTest
    {
        protected HttpClient Client;

        protected ApiTest()
        {
            var factory = new WebApplicationFactory<Program>();

            // special client that only can interact with the factory
            Client = factory.CreateClient();
        }
    }
}
