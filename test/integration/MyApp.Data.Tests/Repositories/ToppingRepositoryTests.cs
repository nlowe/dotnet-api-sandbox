using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApp.Types.Models;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace MyApp.Data.Tests.Repositories
{
    public class ToppingRepositoryTests : DataTest
    {
        [Theory, AutoData]
        public async Task Add(Topping topping)
        {
            await Toppings.Add(topping);

            var result = await Toppings.Get(topping.Id);
            
            Assert.Equal(topping, result);
        }

        [Theory, AutoData]
        public async Task AddManyParams(Topping a, Topping b, Topping c)
        {
            await Toppings.AddMany(a, b, c);

            var result = (await Toppings.GetAll()).ToList();
            
            Assert.Equal(3, result.Count);
            Assert.Contains(a, result);
            Assert.Contains(b, result);
            Assert.Contains(c, result);
        }

        [Theory, AutoData]
        public async Task AddManyEnumerable(List<Topping> toppings)
        {
            await Toppings.AddMany(toppings);

            var result = (await Toppings.GetAll()).ToList();
            
            Assert.Equal(toppings.Count, result.Count);
            Assert.All(result, t => Assert.Contains(t, toppings));
        }

        [Theory, AutoData]
        public async Task GetManyParams(Topping a, Topping b, Topping c)
        {
            await Toppings.AddMany(a, b, c);

            var results = (await Toppings.GetMany(b.Id, c.Id)).ToList();
            
            Assert.Equal(2, results.Count);
            Assert.DoesNotContain(a, results);
            Assert.Contains(b, results);
            Assert.Contains(c, results);
        }

        [Theory, AutoData]
        public async Task GetManyEnumerable(List<Topping> toppings, Topping other)
        {
            await Toppings.AddMany(toppings);
            await Toppings.Add(other);

            var results = (await Toppings.GetMany(toppings.Select(t => t.Id))).ToList();
            
            Assert.Equal(toppings.Count, results.Count);
            Assert.DoesNotContain(other, results);
            Assert.All(results, t => Assert.Contains(t, toppings));
        }

        [Theory, AutoData]
        public async Task Edit(Topping topping, string newName, string newDescription, double newPrice)
        {
            await Toppings.Add(topping);

            topping.Name = newName;
            topping.Description = newDescription;
            topping.Price = newPrice;

            await Toppings.Edit(topping);

            var result = await Toppings.Get(topping.Id);
            
            Assert.Equal(topping, result);
        }

        [Theory, AutoData]
        public async Task DeleteByModel(Topping topping, Topping other)
        {
            await Toppings.AddMany(topping, other);
            await Toppings.Delete(topping);

            var results = (await Toppings.GetAll()).ToList();

            Assert.Single(results);
            Assert.Contains(other, results);
            Assert.DoesNotContain(topping, results);
        }
        
        [Theory, AutoData]
        public async Task DeleteManyByModelEnumerable(List<Topping> toppings, Topping other)
        {
            await Toppings.AddMany(toppings);
            await Toppings.Add(other);
            await Toppings.DeleteMany(toppings);

            var results = (await Toppings.GetAll()).ToList();

            Assert.Single(results);
            Assert.Contains(other, results);
            Assert.All(toppings, t => Assert.DoesNotContain(t, results));
        }
        
        [Theory, AutoData]
        public async Task DeleteManyByModelParams(Topping a, Topping b, Topping other)
        {
            await Toppings.AddMany(a, b);
            await Toppings.Add(other);
            await Toppings.DeleteMany(a, b);

            var results = (await Toppings.GetAll()).ToList();

            Assert.Single(results);
            Assert.Contains(other, results);
            Assert.DoesNotContain(a, results);
            Assert.DoesNotContain(b, results);
        }
        
        [Theory, AutoData]
        public async Task DeleteById(Topping topping, Topping other)
        {
            await Toppings.AddMany(topping, other);
            await Toppings.Delete(topping.Id);

            var results = (await Toppings.GetAll()).ToList();

            Assert.Single(results);
            Assert.Contains(other, results);
            Assert.DoesNotContain(topping, results);
        }
        
        [Theory, AutoData]
        public async Task DeleteManyByIdEnumerable(List<Topping> toppings, Topping other)
        {
            await Toppings.AddMany(toppings);
            await Toppings.Add(other);
            await Toppings.DeleteMany(toppings.Select(t => t.Id));

            var results = (await Toppings.GetAll()).ToList();

            Assert.Single(results);
            Assert.Contains(other, results);
            Assert.All(toppings, t => Assert.DoesNotContain(t, results));
        }
        
        [Theory, AutoData]
        public async Task DeleteManyByIdParams(Topping a, Topping b, Topping other)
        {
            await Toppings.AddMany(a, b);
            await Toppings.Add(other);
            await Toppings.DeleteMany(a.Id, b.Id);

            var results = (await Toppings.GetAll()).ToList();

            Assert.Single(results);
            Assert.Contains(other, results);
            Assert.DoesNotContain(a, results);
            Assert.DoesNotContain(b, results);
        }

        [Theory, AutoData]
        public async Task GetToppingsForPizza(Pizza p)
        {
            await Pizzas.Add(p);

            var results = (await Toppings.GetToppingsForPizza(p)).ToList();
            
            Assert.Equal(p.Toppings.Count, results.Count);
            Assert.All(results, t => Assert.Contains(t, p.Toppings));
        }
    }
}