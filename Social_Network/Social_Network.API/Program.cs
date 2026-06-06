using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;
using Social_Network.API.Services;

namespace Social_Network.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=Socialnetwork.db"));

            builder.Services.AddSignalR();
            builder.Services.AddCors(option => option.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            ));

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
            builder.Services.AddScoped<IChatService,ChatService>();
            builder.Services.AddScoped<IMessageService,MessageService>();
            builder.Services.AddScoped<ILikeService,LikeService>();
            builder.Services.AddScoped<ICommentsService,CommentsService>();
            builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            // Configure the HTTP request pipeline.

            app.UseCors("AllowAll");

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
