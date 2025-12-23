using StaffEventOrganizer.DBContext;
using StaffEventOrganizer.Interface;
using StaffEventOrganizer.Repository;
using StaffEventOrganizer.Middlewares;

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
