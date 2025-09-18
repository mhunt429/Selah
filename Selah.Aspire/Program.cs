var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WebApi>("selah-webapi");
builder.AddProject<Projects.Webhooks_Api>("selah-webhooks-api");

builder.Build().Run();