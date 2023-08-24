using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using SignalRBlazorChatApp.Areas.Identity;
using SignalRBlazorChatApp.Data;
using SignalRBlazorChatApp.Helpers;
using SignalRBlazorChatApp.HttpMethods;
using SignalRBlazorChatApp.Hubs;
using SignalRBlazorChatApp.Models;

namespace SignalRBlazorChatApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Add Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("Logs/SignalBlazorChatApp.txt", rollingInterval: RollingInterval.Month)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("ChatApplicationDb") ?? throw new InvalidOperationException("Connection string 'ChatApplicationDb' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Dependency Injection registration
            builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
            builder.Services.AddScoped<IHubConnectors, HubConnectors>();
            // HTTP Client registration
            builder.Services.AddHttpClient<IPublicChatGroupsApiService, PublicChatGroupsApiService>();
            builder.Services.AddHttpClient<IPublicGroupMessagesApiService, PublicGroupMessagesApiService>();

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
            
            // MudBlazor Snackbar configuration
            builder.Services.AddMudServices(config =>
            {
				config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
				config.SnackbarConfiguration.PreventDuplicates = false;
				config.SnackbarConfiguration.NewestOnTop = false;
				config.SnackbarConfiguration.ShowCloseIcon = true;
				config.SnackbarConfiguration.VisibleStateDuration = 10000;
				config.SnackbarConfiguration.HideTransitionDuration = 500;
				config.SnackbarConfiguration.ShowTransitionDuration = 500;
				config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
			});

            // SignalR configuration
            builder.Services.AddResponseCompression(config =>
            {
                config.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            var app = builder.Build();

            // SignalR
            app.UseResponseCompression();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();

            app.MapBlazorHub();

            // SignalR
            app.MapHub<PublicMessagesHub>("/publicmessageshub");
            app.MapHub<PrivateMessagesHub>("/privatemessageshub");

            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}