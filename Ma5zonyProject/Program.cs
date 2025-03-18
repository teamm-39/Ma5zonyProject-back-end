using DataAccess.Data;
using DataAccess.IRepos;
using DataAccess.Rpos;
using Hangfire;
using Ma5zonyProject.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Models;

namespace Ma5zonyProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //add hangFire
            builder.Services.AddHangfire(h => h.UseSqlServerStorage(builder.Configuration.GetConnectionString("DeafultConnection")));
            builder.Services.AddHangfireServer();
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5173") // Update this to match your front-end URL
                              .AllowCredentials()  // Allow cookies
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            // Configure Application Cookie
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use 'Always' in production
                options.Cookie.SameSite = SameSiteMode.None; // Allow cross-origin cookies
                options.LoginPath = "/api/Users/sign-in"; // Redirect unauthenticated users
                //options.Cookie.IsEssential = true;
            });

            // Configure DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DeafultConnection")));

            // Configure Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Register Repositories
            builder.Services.AddScoped<CustomerIRepo, CustomerRepo>();
            builder.Services.AddScoped<ProductIRepo, ProductRepo>();
            builder.Services.AddScoped<StoreIRepo, StoreRepo>();
            builder.Services.AddScoped<OperationIRepo, OperationRepo>();
            builder.Services.AddScoped<SupplierIRepo, SupplierRepo>();
            builder.Services.AddScoped<StoreProductIRepo, StoreProductRepo>();
            builder.Services.AddScoped<ApplicationUserIRepo, ApplicationUserRepo>();
            builder.Services.AddScoped<OperationStoreProductIRepo, OperationStoreProductRepo>();

            // Configure Identity Options
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHangfireDashboard("/hangfire");
            app.UseHttpsRedirection();
            app.UseStaticFiles(); // Place this once, before UseRouting
            app.UseRouting();
            app.UseCors("AllowFrontend");
            app.UseAuthentication(); // Ensure this is before UseAuthorization
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}