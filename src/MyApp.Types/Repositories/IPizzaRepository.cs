using System;
using MyApp.Types.Models;

namespace MyApp.Types.Repositories
{
    public interface IPizzaRepository : IRepository<Pizza, Guid>
    {
    }
}