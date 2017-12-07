using System.Data;

namespace MyApp.Data.Repositories
{
    public abstract class DatabaseRepository
    {
        protected readonly IDbConnection _db;

        protected DatabaseRepository(IDbConnection db) => _db = db;
    }
}