using Sample_Identity;

namespace SampleMVC_APP
{
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Sample_Identity.Service;

  
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Services.AddControllersWithViews();
                builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = new PathString("/Home/LoginUser");
                        options.AccessDeniedPath = new PathString("/users/AccessDenied");
                    });
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddHttpClient();
                builder.Services.AddScoped<TokenManager>();
                builder.Services.AddScoped<TokenService>();

                builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseRouting();

                app.UseAuthorization();

                app.MapStaticAssets();
                app.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}")
                    .WithStaticAssets();
                app.UseMiddleware<TokenValidationMiddleware>();
                app.Run();
            }
        }
    

}
