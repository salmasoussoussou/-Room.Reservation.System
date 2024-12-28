using nouveaaaaaauuuu.Configuration;
using nouveaaaaaauuuu.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace nouveaaaaaauuuu
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuration de HttpClient pour interagir avec l'API backend
            builder.Services.AddHttpClient("BackendApi", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7165"); //  l'URL
            });


            builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
});

            // Ajouter les services au conteneur.
            builder.Services.AddRazorPages();

            // Ajouter la configuration de l'API
            builder.Services.Configure<ApiSetting>(builder.Configuration.GetSection("ApiSettings"));

            // Ajouter le service UserService au conteneur de services
            builder.Services.AddScoped<IUserService, UserService>();

            // Ajouter les services de session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Durée de la session
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Ajouter le service ApiService pour interagir avec l'API dans Blazor
            builder.Services.AddScoped<ApiService>();
            // Ajouter l'authentification par cookies
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                });
            var app = builder.Build();

            // Configurer le pipeline de requêtes HTTP
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession(); // Utiliser les sessions

            app.UseAuthorization();
            // Utiliser l'authentification et l'autorisation
            app.UseAuthentication();

            app.MapRazorPages();

            app.Run();
        }
    }
}
