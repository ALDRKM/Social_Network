using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;

namespace Social_Network.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=Socialnetwork.db"));

            builder.Services.AddSignalR();
            builder.Services.AddCors(option => option.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            ));

            var app = builder.Build();


            // Configure the HTTP request pipeline.

            app.UseCors("AllowAll");

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
