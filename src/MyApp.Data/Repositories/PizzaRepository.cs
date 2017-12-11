using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MyApp.Types.Models;
using MyApp.Types.Repositories;

namespace MyApp.Data.Repositories
{
    public class PizzaRepository : DatabaseRepository, IPizzaRepository
    {
        private readonly IToppingRepository _toppings;
        
        public PizzaRepository(IDbConnection db, IToppingRepository toppings) : base(db) => _toppings = toppings;
        
        public async Task Add(Pizza model)
        {
            using (var trans = _db.CreateAsyncTransactionScope())
            {
                await _db.ExecuteAsync(
                    "INSERT INTO pizzas VALUES (@id, @name, @description, @basePrice)",
                    new
                    {
                        id = model.Id,
                        name = model.Name,
                        description = model.Description,
                        basePrice = model.BasePrice
                    }
                );

                if (model.Toppings.Count > 0)
                {
                    // TODO: Only add new toppings
                    await _toppings.AddMany(model.Toppings);

                    await _db.ExecuteAsync(
                        "INSERT INTO pizza_toppings VALUES (@pizza, @topping)",
                        model.Toppings.Select(t => new { pizza = model.Id, topping = t.Id })
                    );
                }
                
                trans.Complete();
            }
        }

        public async Task AddMany(params Pizza[] models) => await AddMany(models.AsEnumerable());
        public async Task AddMany(IEnumerable<Pizza> models)
        {
            using (var t = _db.CreateAsyncTransactionScope())
            {
                foreach (var pizza in models)
                {
                    await Add(pizza);
                }
                
                t.Complete();
            }
        }

        public async Task<IEnumerable<Pizza>> GetAll()
        {
            var pizzas = (await _db.QueryAsync<Pizza>("SELECT * from pizzas")).ToList();

            foreach (var p in pizzas)
            {
                p.Toppings.AddRange(await _toppings.GetToppingsForPizza(p));
            }
            
            return pizzas;
        }

        public async Task<Pizza> Get(Guid id)
        {
            var p = await _db.QuerySingleAsync<Pizza>("SELECT * from pizzas WHERE id = @id", new {id});
            p.Toppings.AddRange(await _toppings.GetToppingsForPizza(p));

            return p;
        }

        public async Task<IEnumerable<Pizza>> GetMany(params Guid[] ids) => await GetMany(ids.AsEnumerable());
        public async Task<IEnumerable<Pizza>> GetMany(IEnumerable<Guid> ids)
        {
            var pizzas = (await _db.QueryAsync<Pizza>("SELECT * from pizzas WHERE id = ANY (@ids)", new {ids = ids.ToList()})).ToList();

            foreach (var p in pizzas)
            {
                p.Toppings.AddRange(await _toppings.GetToppingsForPizza(p));
            }
            
            return pizzas;
        }

        public async Task Edit(Pizza model)
        {
            using (var trans = _db.CreateAsyncTransactionScope())
            {
                await _db.ExecuteAsync("DELETE FROM pizza_toppings WHERE pizza = @id", new {id = model.Id});
                
                if (model.Toppings.Count > 0)
                {
                    var toppingsToAdd = model.Toppings.Select(t => t.Id).ToList();

                    var existingToppings = (await _db.QueryAsync<Guid>(
                        "SELECT id FROM toppings WHERE id = ANY (@ids)",
                        new {ids = model.Toppings.Select(t => t.Id).ToList()}
                    )).ToList();

                    toppingsToAdd.RemoveAll(t => existingToppings.Contains(t));

                    await _toppings.AddMany(model.Toppings.Where(t => toppingsToAdd.Contains(t.Id)));
                    foreach (var topping in model.Toppings.Where(t => !toppingsToAdd.Contains(t.Id)))
                    {
                        await _toppings.Edit(topping);
                    }

                    await _db.ExecuteAsync(
                        "INSERT INTO pizza_toppings VALUES (@pizza, @topping)",
                        model.Toppings.Select(t => new { pizza = model.Id, topping = t.Id })
                    );
                }

                await _db.ExecuteAsync(@"
                        UPDATE pizzas SET
                            name = @name,
                            description = @description,
                            basePrice = @basePrice
                        WHERE id = @id
                    ",
                    new { id = model.Id, name = model.Name, description = model.Description, basePrice = model.BasePrice }
                );
                
                trans.Complete();
            }
        }

        public async Task Delete(Pizza model) => await Delete(model.Id);

        public async Task DeleteMany(params Pizza[] models) => await DeleteMany(models.Select(p => p.Id));
        public async Task DeleteMany(IEnumerable<Pizza> models) => await DeleteMany(models.Select(p => p.Id));
        
        public async Task Delete(Guid key)
        {
            using (var t = _db.CreateAsyncTransactionScope())
            {
                var param = new {id = key};
                
                await _db.ExecuteAsync("DELETE FROM pizza_toppings WHERE pizza = @id", param);
                await _db.ExecuteAsync("DELETE FROM pizzas WHERE id = @id", param);
                
                t.Complete();
            }
        }

        public async Task DeleteMany(params Guid[] keys) => await DeleteMany(keys.AsEnumerable());
        public async Task DeleteMany(IEnumerable<Guid> keys)
        {
            using (var t = _db.CreateAsyncTransactionScope())
            {
                var param = new {ids = keys.ToList()};
            
                await _db.ExecuteAsync("DELETE FROM pizza_toppings WHERE pizza = ANY(@ids)", param);
                await _db.ExecuteAsync("DELETE FROM pizzas WHERE id = ANY(@ids)", param);
                
                t.Complete();
            }
        }
    }
}