using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyApp.Types.Models;

namespace MyApp.Types.Repositories
{
    public interface IToppingRepository : IRepository<Topping, Guid>
    {
        Task<IEnumerable<Topping>> GetToppingsForPizza(Pizza p);
    }
}