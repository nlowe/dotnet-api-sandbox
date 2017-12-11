using System.Data;

namespace MyApp.Data.Repositories
{
    public abstract class DatabaseRepository
    {
        protected readonly IDbConnection _db;

        protected DatabaseRepository(IDbConnection db)
        {
            _db = db;

            if(_db.State != ConnectionState.Open)
            {
                _db.Open();
            }
        }
    }
}