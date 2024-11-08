using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleApp;
using XAF.Core.Hosting;
using XAF.Modularity;

var builder = Host.CreateApplicationBuilder();

builder.UseXaf();

builder.Services.AddModuleFromAssembly(typeof(Program).Assembly);
builder.Services.AddHostedService<HostedService>();

var app = builder.Build();

await app.RunAsync();