using Akka.Hosting;
using Akka.Persistence.Sql.Config;
using Akka.Persistence.Sql.Hosting;
using AkkaNpgsqlError;
using LinqToDB;
using Microsoft.Extensions.Hosting;

var connectionString =
    "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=postgres";

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddAkka(
    "TEST",
    (akkaConfigurationBuilder, _) =>
    {
        akkaConfigurationBuilder
            .WithSqlPersistence(
                connectionString,
                ProviderName.PostgreSQL15,
                tagStorageMode: TagMode.Csv
            )
            .WithActors(
                (system, registry, resolver) =>
                {
                    var props = resolver.Props<TestPersistentActor>();
                    var actor = system.ActorOf(props, "test-actor");
                    registry.Register<TestPersistentActor>(actor);
                }
            );
    }
);

var app = builder.Build();
app.Run();
