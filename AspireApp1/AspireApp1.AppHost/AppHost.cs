var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

// TODO: hook up the frontend project here so it is managed by AspireAppHost

builder.Build().Run();
