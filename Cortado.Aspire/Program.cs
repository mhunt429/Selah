var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WebApi>("Cortado-API");

builder.Build().Run();