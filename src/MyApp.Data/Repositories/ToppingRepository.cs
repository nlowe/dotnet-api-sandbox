using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MyApp.Types.Models;
using MyApp.Types.Repositories;

namespace MyApp.Data.Repositories
{
    public class ToppingRepository : DatabaseRepository, IToppingRepository
    {
        public ToppingRepository(IDbConnection db) : base(db)
        {
        }

        public async Task Add(Topping model)
        {
            using (var transaction = _db.CreateAsyncTransactionScope())
            {
                await _db.ExecuteAsync(
                    "INSERT INTO toppings VALUES (@id, @name, @description, @price)",
                    new { id = model.Id, name = model.Name, description = model.Description, price = model.Price}
                );
                
                transaction.Complete();
            }
        }

        public async Task AddMany(params Topping[] models) => await AddMany(models.AsEnumerable());
        public async Task AddMany(IEnumerable<Topping> models)
        {
            using (var transaction = _db.CreateAsyncTransactionScope())
            {
                await _db.ExecuteAsync(
                    "INSERT INTO toppings VALUES (@id, @name, @description, @price)",
                    models.Select(t => new { id = t.Id, name = t.Name, description = t.Description, price = t.Price})
                );
                
                transaction.Complete();
            }
        }

        public async Task<IEnumerable<Topping>> GetAll() => await _db.QueryAsync<Topping>("SELECT * FROM toppings");

        public async Task<Topping> Get(Guid id) => await _db.QuerySingleAsync<Topping>(
            "SELECT * FROM toppings WHERE id = @id",
            new {id}
        );

        public async Task<IEnumerable<Topping>> GetMany(params Guid[] ids) => await GetMany(ids.AsEnumerable());
        public async Task<IEnumerable<Topping>> GetMany(IEnumerable<Guid> ids) => await _db.QueryAsync<Topping>(
            "SELECT * FROM toppings WHERE id = ANY (@ids)",
            new {ids = ids.ToList()}
        );

        public async Task Edit(Topping model) => await _db.ExecuteAsync(
            "UPDATE toppings SET name = @name, description = @description, price = @price WHERE id = @id",
            new { id = model.Id, name = model.Name, description = model.Description, price = model.Price }
        );

        public async Task Delete(Topping model) => await Delete(model.Id);

        public async Task DeleteMany(params Topping[] models) => await DeleteMany(models.Select(t => t.Id));
        public async Task DeleteMany(IEnumerable<Topping> models) => await DeleteMany(models.Select(t => t.Id));

        public async Task Delete(Guid key)
        {
            using (var transaction = _db.CreateAsyncTransactionScope())
            {
                await _db.ExecuteAsync("DELETE FROM pizza_toppings WHERE topping = @key", new {key});
                await _db.ExecuteAsync("DELETE FROM toppings WHERE id = @key", new {key});
                
                transaction.Complete();
            }
        }

        public async Task DeleteMany(params Guid[] keys) => await DeleteMany(keys.AsEnumerable());
        public async Task DeleteMany(IEnumerable<Guid> keys)
        {
            using (var transaction = _db.CreateAsyncTransactionScope())
            {
                var param = new {keys = keys.ToList()};
                    
                await _db.ExecuteAsync("DELETE FROM pizza_toppings WHERE topping = ANY (@keys)", param);
                await _db.ExecuteAsync("DELETE FROM toppings WHERE id = ANY (@keys)", param);
                
                transaction.Complete();
            }
        }

        public async Task<IEnumerable<Topping>> GetToppingsForPizza(Pizza p) => await _db.QueryAsync<Topping>(
            @"SELECT
              t.id,
              t.name,
              t.description,
              t.price
            FROM toppings t
              LEFT JOIN pizza_toppings pt
                ON t.id = pt.topping
            WHERE pt.pizza = @p",
            new {p = p.Id}
        );
    }
}