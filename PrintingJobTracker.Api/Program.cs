using PrintingJobTracker.Api.Data;
using PrintingJobTracker.Api.Extensions;
using PrintingJobTracker.Application.Hubs;
using PrintingJobTracker.Infrastructure;
using PrintingJobTracker.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();
builder.Services.AddSingleton<WeatherForecastService>();

// Dependencys injection of Infrastructure project
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

await app.ApplyMigration();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapHub<AppHub>("/hub/app");

_ = Task.Run(async () =>
{
    var service = app.Services.GetRequiredService<SeedInitializerService>();
    await service.SeedAllAsync();
});

app.Run();
