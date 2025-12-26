using EventOrganizer.DBContext;
using EventOrganizer.Interface;
using EventOrganizer.Middlewares;
using EventOrganizer.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

// Register dependencies
builder.Services.AddSingleton<DapperDbContext>();
builder.Services.AddTransient<IUser, UserRepository>();
builder.Services.AddTransient<IVendor, VendorRepository>();
builder.Services.AddTransient<IOrder, OrderRepository>();
builder.Services.AddTransient<IPackageEvent, PackageEventRepository>();
builder.Services.AddTransient<IVendorConfirmation, VendorConfirmationRepository>();
builder.Services.AddTransient<IPackagePhoto, PackagePhotoRepository>();
builder.Services.AddTransient<ICategory, CategoryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

// Custom Middleware Authorization
app.UseMiddleware<AuthorizationMiddleware>();

app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LandingPage}/{action=Index}/{id?}");

app.Run();
