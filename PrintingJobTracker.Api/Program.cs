using PrintingJobTracker.Api.Extensions;
using PrintingJobTracker.Application;
using PrintingJobTracker.Infrastructure;
using PrintingJobTracker.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Dependencys injection of Infrastructure project
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

await app.ApplyMigration();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

_ = Task.Run(async () =>
{
    var service = app.Services.GetRequiredService<SeedInitializerService>();
    await service.SeedAllAsync();
});

app.Run();