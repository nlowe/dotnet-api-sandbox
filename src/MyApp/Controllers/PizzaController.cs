﻿using System;
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
    [Route("api/v1/pizza")]
    public class PizzaController : Controller
    {
        private readonly IPizzaRepository _pizzas;

        public PizzaController(IPizzaRepository pizzas) =>
            _pizzas = pizzas ?? throw new ArgumentNullException(nameof(pizzas));

        [HttpGet("")]
        public async Task<IEnumerable<Pizza>> GetAll() => await _pizzas.GetAll();

        [HttpGet("{id}")]
        public async Task<Pizza> Get(Guid id) => await _pizzas.Get(id);

        [HttpPost("")]
        public async Task Add([FromBody] Pizza p)
        {
            try
            {
                await _pizzas.Add(p);
                Response.StatusCode = (int) HttpStatusCode.Created;
            }
            catch (PostgresException e)
            {
                if (e.SqlState == "23505")
                {
                    throw new ApiException(HttpStatusCode.Conflict, $"The pizza identified by {p.Id} already exists");
                }

                throw;
            }
        }
        
        [HttpPut("{id}")]
        public async Task Edit(Guid id, [FromBody] Pizza p)
        {
            if (id != p.Id)
            {
                throw new ApiException(HttpStatusCode.BadRequest, $"Tried to edit {id} but got a model for {p.Id}");
            }

            await _pizzas.Edit(p);
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id) => await _pizzas.Delete(id);
    }
}