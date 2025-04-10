using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using MvcDHProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

namespace MvcDHProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddScoped<ICustomerDal, CustomerSqlDal>();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            //          ?? Environment.GetEnvironmentVariable("DATABASE_URL");

            builder.Services.AddDbContext<MVCCoreDbContext>(options =>
                options.UseNpgsql(connectionString));


            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<MVCCoreDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
                .AddGoogle(options =>
                {
                    options.ClientId = Environment.GetEnvironmentVariable("GoogleClientId") ?? throw new InvalidOperationException("GoogleClientId is not set.");
                    options.ClientSecret = Environment.GetEnvironmentVariable("GoogleClientSecret") ?? throw new InvalidOperationException("GoogleClientSecret is not set.");
                    options.Events.OnRedirectToAuthorizationEndpoint = context =>
                    {
                        context.Response.Redirect(context.RedirectUri.Replace("http://", "https://"));
                        return Task.CompletedTask;
                    };
                })
                .AddFacebook(options =>
                {
                    options.AppId = Environment.GetEnvironmentVariable("FacebookAppId") ?? throw new InvalidOperationException("FacebookAppId is not set.");
                    options.AppSecret = Environment.GetEnvironmentVariable("FacebookAppSecret") ?? throw new InvalidOperationException("FacebookAppSecret is not set.");
                });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseStatusCodePagesWithRedirects("/ClientError/{0}");
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            // ✅ TEST DB CONNECTION
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MVCCoreDbContext>();
                try
                {
                    if (db.Database.CanConnect())
                    {
                        Console.WriteLine("✅ Database connected successfully!");
                    }
                    else
                    {
                        Console.WriteLine("❌ Database connection failed.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ DB Connection error: {ex.Message}");
                }
            }

            Console.WriteLine($"Google Client ID: {Environment.GetEnvironmentVariable("GoogleClientId")}");
            Console.WriteLine($"Facebook App ID: {Environment.GetEnvironmentVariable("FacebookAppId")}");

            app.Run();
        }
    }
}
