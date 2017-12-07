using System.IO;

Task("AddMigration")
    .Does(() => 
{
    var name = Argument<string>("name");
    var prefix = DateTime.Now.ToString("yyyyMMddHHmmss");

    EnsureDirectoryExists("src/MyApp.Data/Migrations/src");

    var script = "src/MyApp.Data/Migrations/src/" + prefix + "_" + name + ".sql";
    var cs = "src/MyApp.Data/Migrations/" + prefix + "_" + name + ".cs";

    // Backing Script
    if(!System.IO.File.Exists(script))
    {
        System.IO.File.Create(script);        
    }

    System.IO.File.WriteAllText(cs, $@"using SimpleMigrations;

namespace MyApp.Data.Migrations
{{
    [Migration({prefix}, ""{name}"")]
    public class {name} : ScriptMigration
    {{
        public {name}() : base(""{prefix + "_" + name + ".sql"}"")
        {{
        }}
    }}
}}
");
});