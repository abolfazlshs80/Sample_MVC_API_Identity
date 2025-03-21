
namespace SampleAPI_APP
{


    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using System.Text;

    using Microsoft.AspNetCore.Identity;

    using SampleAPI_APP.Database.Context;
    using SampleDomain.Database.Models;


    public class Program
    {
        public void r()
        {

        }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);



            builder.Services.AddControllers();
            builder.Services.AddSingleton<JwtService>();

            #region Add Identity


            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // تنظیمات اعتبارسنجی رمز عبور
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;

                // تنظیمات قفل کردن حساب کاربری
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                // تنظیمات کوکی‌ها و احراز هویت
                options.SignIn.RequireConfirmedEmail = false;
            });
            builder.Services.AddScoped<RoleManager<ApplicationRole>>();
            // افزودن Authentication و Authorization
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();


            #endregion


            #region Config JWT

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            #endregion


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();

            #region SwaggerGen
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Endpoint iran", Version = "v1" });
                c.EnableAnnotations();


                #region Craete Button Enter And Save JWT  For Swagger


                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "برای ورود، مقدار 'Bearer YOUR_TOKEN' را وارد کنید."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
                #endregion

            });
            #endregion

            builder.Services.AddDbContext<AppDbContext>();





            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //   app.MapOpenApi();

            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Endpoint iran v1");
                c.RoutePrefix = string.Empty;
            });// Swagger در مسیر root نمایش داده شود);
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMiddleware<TokenBlacklistMiddleware>();
            app.UseAuthorization();



            app.MapControllers();



            app.Run();



        }


    }




}
