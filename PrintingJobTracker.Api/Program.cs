using PrintingJobTracker.Api.Extensions;
using PrintingJobTracker.Application;
using PrintingJobTracker.Infrastructure;
using PrintingJobTracker.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

await app.ApplyMigration();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

_ = Task.Run(async () =>
{
    var service = app.Services.GetRequiredService<SeedInitializerService>();
    await service.SeedAllAsync();
});

app.Run();
