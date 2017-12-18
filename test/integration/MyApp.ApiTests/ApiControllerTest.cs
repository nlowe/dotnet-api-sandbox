using System;
using System.Data;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyApp.Types.Repositories;

namespace MyApp.ApiTests
{
    public class ApiControllerTest : IDisposable
    {
        protected readonly TestServer Server;
        protected readonly HttpClient Client;

        protected readonly Mock<IPizzaRepository> Pizzas;
        protected readonly Mock<IToppingRepository> Toppings;
        
        public ApiControllerTest()
        {
            Pizzas = new Mock<IPizzaRepository>();
            Toppings = new Mock<IToppingRepository>();
            
            Server = new TestServer(
                new WebHostBuilder().UseStartup<Startup>()
                    .ConfigureServices(services =>
                    {
                        // Use our DB Connection
                        services.AddSingleton<IDbConnection>(s => null);
                        services.AddSingleton(Toppings.Object);
                        services.AddSingleton(Pizzas.Object);
                    }));
            Client = Server.CreateClient();
        }
        
        public void Dispose()
        {
            // By not calling Complete() on the transaction scope, we roll it back
            Client.Dispose();
            Server.Dispose();
        }     
    }
}