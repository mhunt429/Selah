var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WebApi>("selah-webapi");

builder.Build().Run();