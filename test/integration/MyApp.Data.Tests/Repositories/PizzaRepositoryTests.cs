using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApp.Types.Models;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace MyApp.Data.Tests.Repositories
{
    public class PizzaRepositoryTests : DataTest
    {
        [Theory, AutoData]
        public async Task Add(Pizza pizza)
        {
            await Pizzas.Add(pizza);

            var response = await Pizzas.Get(pizza.Id);
            
            Assert.Equal(pizza, response);
        }

        [Theory, AutoData]
        public async Task Add_NoToppings(Pizza p)
        {
            p.Toppings.Clear();
            await Pizzas.Add(p);

            var response = await Pizzas.Get(p.Id);
            
            Assert.Equal(p, response);
            Assert.Empty(await Toppings.GetAll());
        }
        
        // TODO: Add only adds new toppings

        [Theory, AutoData]
        public async Task AddManyByParam(Pizza a, Pizza b, Pizza c)
        {
            await Pizzas.AddMany(a, b, c);

            var results = (await Pizzas.GetAll()).ToList();
            
            Assert.Equal(3, results.Count);
            Assert.Contains(a, results);
            Assert.Contains(b, results);
            Assert.Contains(c, results);
        }

        [Theory, AutoData]
        public async Task AddManyByEnumerable(List<Pizza> ps)
        {
            await Pizzas.AddMany(ps);

            var results = (await Pizzas.GetAll()).ToList();
            
            Assert.Equal(ps.Count, results.Count);
            Assert.All(ps, p => Assert.Contains(p, results));
        }

        [Theory, AutoData]
        public async Task GetManyByParam(Pizza a, Pizza b, Pizza c)
        {
            await Pizzas.AddMany(a, b, c);

            var results = (await Pizzas.GetMany(a.Id, c.Id)).ToList();
            
            Assert.Equal(2, results.Count);
            Assert.Contains(a, results);
            Assert.DoesNotContain(b, results);
            Assert.Contains(c, results);
        }

        [Theory, AutoData]
        public async Task GetManyByEnumerable(List<Pizza> ps, Pizza exclude)
        {
            await Pizzas.Add(exclude);
            await Pizzas.AddMany(ps);

            var results = (await Pizzas.GetMany(ps.Select(p => p.Id))).ToList();
            
            Assert.Equal(ps.Count, results.Count);
            Assert.All(ps, p => Assert.Contains(p, results));
            Assert.DoesNotContain(exclude, results);
        }

        [Theory, AutoData]
        public async Task Edit_NoToppings(Pizza p, Pizza changes)
        {
            await Pizzas.Add(p);

            p.BasePrice = changes.BasePrice;
            p.Description = changes.Description;
            p.Name = changes.Name;
            p.Toppings.Clear();

            await Pizzas.Edit(p);

            var result = await Pizzas.Get(p.Id);
            
            Assert.Equal(p, result);
        }

        [Theory, AutoData]
        public async Task Edit_ChangedToppings(Pizza p, Pizza changes)
        {
            await Pizzas.Add(p);

            p.BasePrice = changes.BasePrice;
            p.Description = changes.Description;
            p.Name = changes.Name;
            p.Toppings.AddRange(changes.Toppings);

            await Pizzas.Edit(p);

            var result = await Pizzas.Get(p.Id);
            
            Assert.Equal(p, result);
        }

        [Theory, AutoData]
        public async Task DeleteByModel(Pizza p)
        {
            await Pizzas.Add(p);
            await Pizzas.Delete(p);
            
            Assert.Empty(await Pizzas.GetAll());

            var toppings = (await Toppings.GetAll()).ToList();
            Assert.Equal(p.Toppings.Count, toppings.Count);
            Assert.All(toppings, t => Assert.Contains(t, p.Toppings));
        }
        
        [Theory, AutoData]
        public async Task DeleteById(Pizza p)
        {
            await Pizzas.Add(p);
            await Pizzas.Delete(p.Id);
            
            Assert.Empty(await Pizzas.GetAll());

            var toppings = (await Toppings.GetAll()).ToList();
            Assert.Equal(p.Toppings.Count, toppings.Count);
            Assert.All(toppings, t => Assert.Contains(t, p.Toppings));
        }

        [Theory, AutoData]
        public async Task DeleteManyByParamsModel(Pizza a, Pizza b, Pizza c)
        {
            await Pizzas.AddMany(a, b, c);
            await Pizzas.DeleteMany(a, c);

            var ps = (await Pizzas.GetAll()).ToList();
            var ts = (await Toppings.GetAll()).ToList();

            var expectedToppings = a.Toppings.Concat(b.Toppings).Concat(c.Toppings).ToList();
            
            Assert.Equal(1, ps.Count);
            Assert.Contains(b, ps);
            
            Assert.Equal(expectedToppings.Count, ts.Count);
            Assert.All(ts, t => Assert.Contains(t, expectedToppings));
        }
        
        [Theory, AutoData]
        public async Task DeleteManyByEnumerableModel(List<Pizza> pizzas, Pizza exclude)
        {
            await Pizzas.Add(exclude);
            await Pizzas.AddMany(pizzas);
            await Pizzas.DeleteMany(pizzas);

            var ps = (await Pizzas.GetAll()).ToList();
            var ts = (await Toppings.GetAll()).ToList();

            var expectedToppings = pizzas.SelectMany(p => p.Toppings).Concat(exclude.Toppings).ToList();
            
            Assert.Equal(1, ps.Count);
            Assert.Contains(exclude, ps);
            
            Assert.Equal(expectedToppings.Count, ts.Count);
            Assert.All(ts, t => Assert.Contains(t, expectedToppings));
        }
        
        [Theory, AutoData]
        public async Task DeleteManyByParamsId(Pizza a, Pizza b, Pizza c)
        {
            await Pizzas.AddMany(a, b, c);
            await Pizzas.DeleteMany(a.Id, c.Id);

            var ps = (await Pizzas.GetAll()).ToList();
            var ts = (await Toppings.GetAll()).ToList();

            var expectedToppings = a.Toppings.Concat(b.Toppings).Concat(c.Toppings).ToList();
            
            Assert.Equal(1, ps.Count);
            Assert.Contains(b, ps);
            
            Assert.Equal(expectedToppings.Count, ts.Count);
            Assert.All(ts, t => Assert.Contains(t, expectedToppings));
        }
        
        [Theory, AutoData]
        public async Task DeleteManyByEnumerableId(List<Pizza> pizzas, Pizza exclude)
        {
            await Pizzas.Add(exclude);
            await Pizzas.AddMany(pizzas);
            await Pizzas.DeleteMany(pizzas.Select(p => p.Id));

            var ps = (await Pizzas.GetAll()).ToList();
            var ts = (await Toppings.GetAll()).ToList();

            var expectedToppings = pizzas.SelectMany(p => p.Toppings).Concat(exclude.Toppings).ToList();
            
            Assert.Equal(1, ps.Count);
            Assert.Contains(exclude, ps);
            
            Assert.Equal(expectedToppings.Count, ts.Count);
            Assert.All(ts, t => Assert.Contains(t, expectedToppings));
        }
    }
}