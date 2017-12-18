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
    public class ToppingController : ApiControllerTest
    {
        [Theory, ValidAutoData]
        public async Task GetMany(List<Topping> tps)
        {
            Toppings.Setup(ts => ts.GetAll()).ReturnsAsync(tps);

            var response = (await Client.GetJsonAsync<IEnumerable<Topping>>("api/v1/topping")).ToList();

            Assert.Equal(tps.Count, response.Count);
            Assert.All(response, t => Assert.Contains(t, tps));
        }
        
        [Theory, ValidAutoData]
        public async Task GetById(Topping t)
        {
            Toppings.Setup(ts => ts.Get(It.IsAny<Guid>())).ReturnsAsync(t);
            
            var response = await Client.GetJsonAsync<Topping>($"api/v1/topping/{t.Id}");
            
            Assert.Equal(t, response);
            Toppings.Verify(ts => ts.Get(t.Id), Times.Once);
        }
        
        [Theory, ValidAutoData]
        public async Task Add(Topping t)
        {
            var response = await Client.PostJsonAsync("api/v1/topping", t);
            
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Toppings.Verify(ts => ts.Add(t), Times.Once);
        }

        [Theory, ValidAutoData]
        public async Task ConflictForDuplicate(Topping t)
        {
            Toppings.Setup(ts => ts.Add(It.IsAny<Topping>())).ThrowsAsync(new PostgresException {SqlState = "23505"});

            var response = await Client.PostJsonAsync("api/v1/topping", t);
            
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            var error = JsonConvert.DeserializeObject<ApiExceptionMessage>(await response.Content.ReadAsStringAsync());
            Assert.Equal($"The topping identified by {t.Id} already exists", error.Message);
            
            Toppings.Verify(ps => ps.Add(t), Times.Once);
        }

        [Theory, ValidAutoData]
        public async Task Edit(Topping t)
        {
            var response = await Client.PutJsonAsync($"api/v1/topping/{t.Id}", t);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Toppings.Verify(ts => ts.Edit(t), Times.Once);
        }

        [Theory, ValidAutoData]
        public async Task BadRequestForNonMatchingId(Topping t, Guid id)
        {
            var response = await Client.PutJsonAsync($"api/v1/topping/{id}", t);
            
            var error = JsonConvert.DeserializeObject<ApiExceptionMessage>(await response.Content.ReadAsStringAsync());
            Assert.Equal($"Tried to edit {id} but got a model for {t.Id}", error.Message);
            Toppings.Verify(ts => ts.Edit(It.IsAny<Topping>()), Times.Never);
        }

        [Theory, ValidAutoData]
        public async Task Delete(Topping t)
        {
            var response = await Client.DeleteAsync($"api/v1/topping/{t.Id}");
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Toppings.Verify(ts => ts.Delete(t.Id), Times.Once);
        }
    }
}