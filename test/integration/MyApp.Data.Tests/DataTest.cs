using System;
using System.Data;
using System.Transactions;
using MyApp.Data.Repositories;
using Npgsql;

namespace MyApp.Data.Tests
{
    public class DataTest : IDisposable
    {
        public const string DEFAULT_CONNECTION_STRING = "Server=localhost;User ID=postgres;Database=postgres";

        private static readonly string ConnectionString = Environment.GetEnvironmentVariable("MYAPP_DATA_TESTS_CON") ?? DEFAULT_CONNECTION_STRING;

        private readonly IDbConnection _db;

        protected readonly PizzaRepository Pizzas;
        protected readonly ToppingRepository Toppings;

        private readonly TransactionScope _scope;

        static DataTest()
        {
            new DatabaseMigrator(ConnectionString).Migrate();            
        }

        public DataTest()
        {
            _db = new NpgsqlConnection(ConnectionString);
            if (_db.State != ConnectionState.Open)
            {
                _db.Open();
            }
            
            _scope = _db.CreateAsyncTransactionScope();
            
            Toppings = new ToppingRepository(_db);
            Pizzas = new PizzaRepository(_db, Toppings);
        }

        public void Dispose()
        {
            // By not calling Complete() on the transaction scope, we roll it back
            _scope.Dispose();
            _db.Dispose();
        }     
    }
}