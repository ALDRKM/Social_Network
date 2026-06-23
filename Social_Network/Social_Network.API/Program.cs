using Microsoft.EntityFrameworkCore;
using Social_Network.API.Data;
using Social_Network.API.Services;
using Social_Network.API.Hubs;

namespace Social_Network.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(o =>
                o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
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
            builder.Services.AddScoped<ISavedPostService,SavedPostService>();
            builder.Services.AddScoped<ICommentsService,CommentsService>();
            builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();
            builder.Services.AddScoped<IReportService, ReportService>();

            var app = builder.Build();

            // Применяем миграции при старте, чтобы схема БД всегда была актуальной
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            // Папка для загруженных изображений (доступна по сети с любого устройства)
            var webRoot = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
            Directory.CreateDirectory(Path.Combine(webRoot, "uploads"));
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(webRoot)
            });

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
            app.MapHub<ChatHub>("/hubs/chat");

            app.Run();
        }
    }
}
