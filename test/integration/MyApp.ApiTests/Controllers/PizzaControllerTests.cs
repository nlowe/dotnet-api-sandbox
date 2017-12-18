using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Moq;
using MyApp.ApiTests.Extensions;
using MyApp.Types.Models;
using Newtonsoft.Json;
using Npgsql;
using Xunit;

namespace MyApp.ApiTests.Controllers
{
    public class PizzaControllerTests : ApiControllerTest
    {
        [Theory, ValidAutoData]
        public async Task GetMany(List<Pizza> pzs)
        {
            Pizzas.Setup(ps => ps.GetAll()).ReturnsAsync(pzs);

            var response = (await Client.GetJsonAsync<IEnumerable<Pizza>>("api/v1/pizza")).ToList();
            
            Assert.Equal(pzs.Count, response.Count);
            Assert.All(response, p => Assert.Contains(p, pzs));
        }
        
        [Theory, ValidAutoData]
        public async Task GetById(Pizza p)
        {
            Pizzas.Setup(ps => ps.Get(It.IsAny<Guid>())).ReturnsAsync(p);            

            var pizza = await Client.GetJsonAsync<Pizza>($"api/v1/pizza/{p.Id}");
            
            Assert.Equal(p, pizza);
            Pizzas.Verify(ps => ps.Get(p.Id), Times.Once);
        }
        
        [Theory, ValidAutoData]
        public async Task Add(Pizza p)
        {
            var response = await Client.PostJsonAsync("api/v1/pizza", p);
            
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            Pizzas.Verify(ps => ps.Add(p), Times.Once);
        }

        [Theory, ValidAutoData]
        public async Task ConflicForDuplicateAdd(Pizza p)
        {
            Pizzas.Setup(ps => ps.Add(It.IsAny<Pizza>())).ThrowsAsync(new PostgresException {SqlState = "23505"});
            var response = await Client.PostJsonAsync("api/v1/pizza", p);
            
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            var error = JsonConvert.DeserializeObject<ApiExceptionMessage>(await response.Content.ReadAsStringAsync());
            Assert.Equal($"The pizza identified by {p.Id} already exists", error.Message);
            
            Pizzas.Verify(ps => ps.Add(p), Times.Once);
        }

        [Theory, ValidAutoData]
        public async Task Edit(Pizza p)
        {
            await Client.PostJsonAsync("api/v1/pizza", p);

            var response = await Client.PutJsonAsync($"api/v1/pizza/{p.Id}", p);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Pizzas.Verify(ps => ps.Edit(p), Times.Once);
        }

        [Theory, ValidAutoData]
        public async Task BadRequestForNonMatchingId(Pizza p, Guid id)
        {
            var response = await Client.PutJsonAsync($"api/v1/pizza/{id}", p);

            var error = JsonConvert.DeserializeObject<ApiExceptionMessage>(await response.Content.ReadAsStringAsync());
            Assert.Equal($"Tried to edit {id} but got a model for {p.Id}", error.Message);
            Pizzas.Verify(ps => ps.Edit(It.IsAny<Pizza>()), Times.Never);
        }

        [Theory, ValidAutoData]
        public async Task Delete(Pizza p)
        {
            var response = await Client.DeleteAsync($"api/v1/pizza/{p.Id}");
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Pizzas.Verify(ps => ps.Delete(p.Id), Times.Once);
        }
    }
}