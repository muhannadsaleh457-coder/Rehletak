
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rehletak.Abstractions;
using Rehletak.Domain.Entites.Auth;
using Rehletak.Presistense.Contexts;
using Rehletak.Services;
using Rehletak.Services.Auth.Options;
using Rehletak.Web.BackgroundJobs;
using Rehletak.Web.Middlewares;
using StackExchange.Redis;
using System.Text;

namespace Rehletak
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Add Swagger/OpenAPI services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddCors(options =>
            {
               
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // ← مهم عشان الـ cookies
                });
            });

            builder.Services.AddIdentityCore<AppUser>()
            .AddSignInManager()
           .AddRoles<IdentityRole>()
           .AddEntityFrameworkStores<RehletakDbContext>()
           .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                    )
                };
            }).AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;  
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            })

                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["Google:ClientId"];
                    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
                    //options.CallbackPath = "/api/auth/google/callback";

                    options.CorrelationCookie.SameSite = SameSiteMode.None;
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.CorrelationCookie.HttpOnly = true;

                    options.SaveTokens = true;

                });

            

            builder.Services.AddScoped<IServiceManager, ServiceManager>();


           


            builder.Services.AddDbContext<RehletakDbContext>(options =>
                options.UseSqlServer(builder.Configuration
                .GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'RehletakContext' not found.")));

            //Redis Connection

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var connectionString = builder.Configuration.GetConnectionString("Redis")
                    ?? "localhost:6379";
                return ConnectionMultiplexer.Connect(connectionString);
            });



            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["Redis:ConnectionString"];
            });

            builder.Services.AddHangfire(config =>
            {
                config.UseRecommendedSerializerSettings()
                .UseSimpleAssemblyNameTypeSerializer()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
            });


            builder.Services.Configure<TwilioOption>(builder.Configuration.GetSection("TwilioSettings"));
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


            var app = builder.Build();


            //Hangfire
            var jobManager = app.Services.GetRequiredService<IRecurringJobManager>();

            jobManager.AddOrUpdate<MyBackGroundJobs>(
                "CleanNotActiveTokens",
                j => j.CleanNotActiveTokens(),
                Cron.Daily
                );

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
                // The default route will be '/swagger'.
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAngular");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<HandleExceptions>();

            app.MapControllers();

            app.Run();
        }
    }
}
