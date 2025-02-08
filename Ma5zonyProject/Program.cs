
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
            builder.Services.AddScoped<ProductExportIRepo, ProductExportRepo>();
            builder.Services.AddScoped<ProductImportIRepo, ProductImportRepo>();
            builder.Services.AddScoped<StoreProductIRepo, StoreProductRepo>();
            builder.Services.AddScoped<UserMangerProductIRepo, UserMangeProductRepo>();
            builder.Services.AddScoped<UserMangerStoreIRepo, UserMangeStoreRepo>();
            builder.Services.AddScoped<ApplicationUserIRepo, ApplicationUserRepo>();
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;       // لازم يكون فيه رقم
                options.Password.RequiredLength = 6;        // الحد الأدنى للطول (غيّره حسب الحاجة)
                options.Password.RequireLowercase = true;  // لا يحتاج حروف صغيرة
                options.Password.RequireUppercase = true;  // لا يحتاج حروف كبيرة
                options.Password.RequireNonAlphanumeric = false; // لا يحتاج رموز خاصة
            });
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
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
