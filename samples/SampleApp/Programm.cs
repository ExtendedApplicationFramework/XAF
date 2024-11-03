using Microsoft.Extensions.Hosting;
using XAF.Core.Hosting;

var builder = Host.CreateApplicationBuilder();

builder.UseXaf();

builder.Services.AddModularity();

var app = builder.Build();

await app.RunAsync();