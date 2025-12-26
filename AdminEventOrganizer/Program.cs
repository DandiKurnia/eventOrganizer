using AdminEventOrganizer.DBContext;
using AdminEventOrganizer.Interface;
using AdminEventOrganizer.Middlewares;
using AdminEventOrganizer.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddTransient<DapperDbContext, DapperDbContext>();
builder.Services.AddTransient<IVendor, VendorRepository>();
builder.Services.AddTransient<IPackageEvent, PackageEventRepository>();
builder.Services.AddTransient<IUser, UserRepository>();
builder.Services.AddTransient<IPackagePhoto, PackagePhotoRepository>();
builder.Services.AddTransient<IOrder, OrderRepository>();
builder.Services.AddTransient<IDashboard, DashboardRepository>();
builder.Services.AddTransient<ICategory, CategoryRepository>();

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
app.UseSession();

app.UseMiddleware<AuthorizationMiddleware>();


app.UseAuthorization();

app.MapControllerRoute(
    name: "root",
    pattern: "",
    defaults: new { controller = "Dashboard", action = "Index" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "/{controller=Pemesanan}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "/{controller=Vendor}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
