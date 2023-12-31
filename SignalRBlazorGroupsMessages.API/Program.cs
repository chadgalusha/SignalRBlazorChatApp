using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Services;
using System.Text;

namespace SignalRBlazorGroupsMessages.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Add Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("Logs/SignalBlazorGroupsMessagesAPI.txt", rollingInterval: RollingInterval.Month)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
            }).AddNewtonsoftJson();

            // Interface to implementing class
            builder.Services.AddScoped<IPublicGroupMessagesDataAccess, PublicGroupMessagesDataAccess>();
            builder.Services.AddScoped<IPrivateGroupMessagesDataAccess, PrivateGroupMessagesDataAccess>();
            builder.Services.AddScoped<IPublicChatGroupsDataAccess, PublicChatGroupsDataAccess>();
            builder.Services.AddScoped<IPrivateChatGroupsDataAccess, PrivateChatGroupsDataAccess>();
            builder.Services.AddScoped<IPublicGroupMessagesService, PublicGroupMessagesService>();
            builder.Services.AddScoped<IPublicChatGroupsService, PublicChatGroupsService>();
            builder.Services.AddScoped<IPrivateGroupMessagesService, PrivateGroupMessagesService>();
            builder.Services.AddScoped<IPrivateChatGroupsService, PrivateChatGroupsService>();
            builder.Services.AddScoped<ISerilogger, Serilogger>();
            builder.Services.AddScoped<IUserProvider, UserProvider>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Db Context connection
            var connectionString = builder.Configuration.GetConnectionString("ChatApplicationDb") ?? throw new InvalidOperationException("Connection string 'ChatApplicationDb' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            // JWT authentication
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Authentication:Issuer"],
                        ValidAudience = builder.Configuration["Authentication:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
                    };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //app.MapControllers();

            app.Run();
        }
    }
}