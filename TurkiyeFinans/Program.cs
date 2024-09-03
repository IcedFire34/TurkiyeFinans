using Microsoft.EntityFrameworkCore;
using TurkiyeFinans.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Baðlantý dizesini appsettings.json'dan alarak DbContext'i ekleyin.
builder.Services.AddDbContext<TurkiyeFinansDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// `appsettings.json`'dan Connection String alýnýyor
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// CustomerOperations için DI ayarý
builder.Services.AddSingleton(new CustomerOperations(connectionString));

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
