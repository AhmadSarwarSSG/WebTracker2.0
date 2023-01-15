using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using WebTracker;
using WebTracker.Hubs;
using WebTracker.Repositories;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// Add services to the container.
string connString = builder.Configuration.GetConnectionString("DefaultConnection");
var migrationAssembly = typeof(Program).Assembly.GetName().Name;
builder.Services.AddTransient<IWebsiteRepository, WebsiteRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IFlowRepository, FlowRepository>();
builder.Services.AddTransient<IActionRepository, ActionRepository>();
builder.Services.AddTransient<IFlowDataRepository, FlowDataRepository>();
builder.Services.AddTransient<ISummaryData, SummaryData>();
builder.Services.AddDbContext<WebTrackerDBContext>(options =>
           options.UseSqlServer(connString, sql => sql.MigrationsAssembly(migrationAssembly)));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<WebTrackerDBContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
                          builder =>
                          {
                              builder.AllowAnyHeader()
                                                  .AllowAnyMethod()
                                                  .WithOrigins("https://localhost:7227/");


                          });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseCors();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ClientHub>("/chatHub");
});
app.Run();
