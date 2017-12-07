using SimpleMigrations;

namespace MyApp.Data.Migrations
{
    [Migration(20171203151711, "InitialState")]
    public class InitialState : ScriptMigration
    {
        public InitialState() : base("20171203151711_InitialState.sql")
        {
        }
    }
}
