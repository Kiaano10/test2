using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.SlidingExpiration = true;
            });

        builder.Services.AddAuthorization();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<JwtAuthorizationHandler>();

        builder.Services.AddHttpClient("HomeAwayFromHomeAPI", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7243/");
            client.Timeout = TimeSpan.FromSeconds(60);
        }).AddHttpMessageHandler<JwtAuthorizationHandler>();

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(1);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        builder.Services.AddScoped<PropertyApiService>();
        builder.Services.AddScoped<AuthApiService>();
        builder.Services.AddScoped<AmenityApiService>();
        builder.Services.AddScoped<AvailabilityApiService>();
        builder.Services.AddScoped<BookingApiService>();
        builder.Services.AddScoped<ReviewApiService>();
        builder.Services.AddScoped<FinancialTransactionApiService>();
        builder.Services.AddScoped<FinancialReportApiService>();
        builder.Services.AddScoped<CustomerApiService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
