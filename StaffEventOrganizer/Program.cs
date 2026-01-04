using StaffEventOrganizer.DBContext;
using StaffEventOrganizer.Interface;
using StaffEventOrganizer.Repository;
using StaffEventOrganizer.Middlewares;
using StaffEventOrganizer.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC + Razor
builder.Services.AddControllersWithViews();

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Routing lowercase
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

// DI
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddTransient<IUser, UserRepository>();
builder.Services.AddTransient<ICategory, CategoryRepository>();
builder.Services.AddTransient<IPackageEvent, PackageEventRepository>();
builder.Services.AddTransient<IPackagePhoto, PackagePhotoRepository>();
builder.Services.AddTransient<IOrder, OrderRepository>();
builder.Services.AddTransient<IDashboard, DashboardRepository>();
builder.Services.AddTransient<IVendor, VendorRepository>();
builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

// ================= PIPELINE =================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseMiddleware<AuthorizationMiddleware>();

app.UseAuthorization();

// Attribute routing
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

app.Run(); // ✅ SATU KALI SAJA
