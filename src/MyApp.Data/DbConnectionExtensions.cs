using System.Data;
using System.Data.Common;
using System.Transactions;

namespace MyApp.Data
{
    public static class DbConnectionExtensions
    {
        public static TransactionScope CreateAsyncTransactionScope(this IDbConnection db)
        {
            var result = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            (db as DbConnection)?.EnlistTransaction(Transaction.Current);

            return result;
        }
    }
}