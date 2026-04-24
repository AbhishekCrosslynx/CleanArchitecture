IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresDatabaseResource> database = builder
    .AddPostgres("database")
    .WithImage("postgres:17")
    .WithBindMount("../../.containers/db", "/var/lib/postgresql/data")
    .AddDatabase("clean-architecture");

IResourceBuilder<ProjectResource> webapi = builder.AddProject<Projects.Web_Api>("WebApi")
    .WithEnvironment("ConnectionStrings__Database", database)
    .WithReference(database)
    .WaitFor(database);

// ----- UI -----
IResourceBuilder<ProjectResource> shared = builder.AddProject<Projects.Shared>("Shared")
    .WithReference(webapi);

builder.AddProject<Projects.Client>("Client")
    .WithReference(shared)
    .WithReference(webapi);

builder.AddProject<Projects.Operations>("Operations")
    .WithReference(shared)
    .WithReference(webapi);

builder.AddProject<Projects.Test>("Test")
    .WithReference(shared)
    .WithReference(webapi);

await builder.Build().RunAsync();
