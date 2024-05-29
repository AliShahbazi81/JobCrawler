using JobScrawler.ExceptionHandling;
using JobScrawler.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOtherServices(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

//! Seeding Database
using var scope = app.Services.CreateScope();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
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