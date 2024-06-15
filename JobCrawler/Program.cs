using JobCrawler.Data.Crawler.Context;
using JobCrawler.Data.Crawler.Initializer;
using JobScrawler.ExceptionHandling;
using JobScrawler.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddOtherServices(builder.Configuration);
builder.Services.AddAuthenticationService(builder.Environment);

builder.Services.AddControllers();

var app = builder.Build();

using var scope = app.Services.CreateScope();

app.EnsureDatabaseMigrated();

//! Seeding Database
var context = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();

try
{
    await DbInitializer.Initialize(context);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("cors");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllerRoute(
    "default",
    "{controller}/{action=Index}/{id?}");

app.MapControllers();

app.Run();