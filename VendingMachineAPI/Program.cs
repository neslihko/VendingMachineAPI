using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VendingMachineAPI.Helpers;
using VendingMachineAPI.Services;

namespace VendingMachineAPI
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var appSection = builder.Configuration.GetSection("AppSettings");
            var appSettings = appSection.Get<AppSettings>();

            builder.Services.AddDbContext<DataContext>(x => x.UseInMemoryDatabase("TestDb"));

            // Access other services if needed
            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    // Access the ApplicationDbContext and perform migrations
                    var dbContext = services.GetRequiredService<DataContext>();
                    dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    // Handle exceptions if any during migration
                    Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
                }
            }

            // Register your other services
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IVendingMachineService, VendingMachineService>();

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = appSettings.Issuer,

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Key))
                    };
                });

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}