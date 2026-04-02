
using Microsoft.EntityFrameworkCore;
using Rehletak.Presistense.Contexts;

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

            builder.Services.AddDbContext<RehletakDbContext>(options =>
                options.UseSqlServer(builder.Configuration
                .GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'RehletakContext' not found.")));

            var app = builder.Build();

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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
