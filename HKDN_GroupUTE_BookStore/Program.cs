using HKDN_GroupUTE_BookStore.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CsharpBookShopContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("BookShopDB"),
        ServerVersion.Parse("10.4.32-mariadb")
    )
);

//add session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//add session
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index_Home}/{id?}");

app.Run();
