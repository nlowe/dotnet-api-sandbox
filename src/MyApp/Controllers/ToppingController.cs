using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyApp.Exceptions;
using MyApp.Types.Models;
using MyApp.Types.Repositories;
using Npgsql;

namespace MyApp.Controllers
{
    [Route("api/v1/topping")]
    public class ToppingController : Controller
    {
        private readonly IToppingRepository _toppings;

        public ToppingController(IToppingRepository toppings) =>
            _toppings = toppings ?? throw new ArgumentNullException(nameof(toppings));

        [HttpGet("")]
        public async Task<IEnumerable<Topping>> GetAll() => await _toppings.GetAll();

        [HttpGet("{id}")]
        public async Task<Topping> Get(Guid id) => await _toppings.Get(id);
        
        [HttpPost("")]
        public async Task Add([FromBody] Topping t)
        {   
            try
            {
                await _toppings.Add(t);
            }
            catch (PostgresException e)
            {
                if (e.SqlState == "23505")
                {
                    throw new ApiException(HttpStatusCode.Conflict, $"The topping identified by {t.Id} already exists");
                }

                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task Edit(Guid id, [FromBody] Topping t)
        {
            if (id != t.Id)
            {
                throw new ArgumentException($"Tried to edit {id} but got a model for {t.Id}", nameof(id));
            }

            await _toppings.Edit(t);
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id) => await _toppings.Delete(id);
    }
}