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
app.MapControllers();

// ROOT → /login
app.MapControllerRoute(
    name: "root",
    pattern: "",
    defaults: new { controller = "User", action = "Login" }
);

// DEFAULT
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}"
);



app.Run();
