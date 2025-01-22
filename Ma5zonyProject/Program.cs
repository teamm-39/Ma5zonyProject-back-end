
using DataAccess.Data;
using DataAccess.IRepos;
using DataAccess.Rpos;
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

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //builder.services.addscoped
            //DbContext
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()   // يسمح بأي أصل (يمكنك تخصيصه بنطاقات معينة)
                          .AllowAnyMethod()   // يسمح بأي نوع من الطلبات (GET, POST, PUT, DELETE, إلخ)
                          .AllowAnyHeader();  // يسمح بأي رأس
                });
            });
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DeafultConnection")));
            //1-Configure Identity User&Roles
            //
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddScoped<CustomerIRepo, CustomerRepo>();
            builder.Services.AddScoped<ExportIRepo, ExportRepo>();
            builder.Services.AddScoped<ImportIRepo, ImportRepo>();
            builder.Services.AddScoped<ProductIRepo, ProductRepo>();
            builder.Services.AddScoped<StoreIRepo, StoreRepo>();
            builder.Services.AddScoped<SupplierIRepo, SupplierRepo>();
            //
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()||app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
