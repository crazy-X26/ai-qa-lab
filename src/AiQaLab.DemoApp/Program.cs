using AiQaLab.AI.Services;
using AiQaLab.AI.Services.Interfaces;
using AiQaLab.DemoApp.Services;
using AiQaLab.DemoApp.Services.Interfaces;

namespace AiQaLab.DemoApp
{
    public partial class Program
    {
        
        public static void Main(string[] args)
        {
            var app = CreateApp(args);

            app.Run();
        }

        public static WebApplication CreateApp(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                ApplicationName = typeof(Program).Assembly.GetName().Name
            });

            builder.Configuration.AddUserSecrets<Program>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IUserSessionService, UserSessionService>();
            
            var geminiApiKey = builder.Configuration["AI:Gemini:ApiKey"];
            if (string.IsNullOrEmpty(geminiApiKey))
            {
                throw new InvalidOperationException("Gemini API key is not configured. Please set the 'AI:Gemini:ApiKey' configuration value.");
            }

            builder.Services.AddSingleton<IAIClient>(_ => new GeminiAIClient(geminiApiKey));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            return app;
        }
    }
}
