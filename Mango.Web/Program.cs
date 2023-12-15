using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<ICartService, CartService>();

builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddHttpClient<IAuthService, AuthService>();

builder.Services.AddScoped<IProductsService, ProductService>(); 
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddHttpClient<IProductService,CouponService>();
builder.Services.AddHttpClient<IOrderService,OrderService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();


SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];
SD.OrderAPI = builder.Configuration["ServiceUrls:OrderAPI"];

SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];
SD.ShoppingCartAPI = builder.Configuration["ServiceUrls:ShoppingCartAPI"];


builder.Services.AddScoped<IProductService, CouponService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(Options =>
    {
        Options.ExpireTimeSpan = TimeSpan.FromHours(10);
        Options.LoginPath = "/Auth/Login";
        Options.AccessDeniedPath = "/auth/AccessDenied";
    });
    // Other cookie authentication options...
      
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
