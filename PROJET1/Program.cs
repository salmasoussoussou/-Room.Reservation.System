
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using projet1.Data;
using projet1.Extention;
using projet1.models;

var builder = WebApplication.CreateBuilder(args);

//
builder.Services.AddControllersWithViews();
//
// Configurer le contexte de la base de données
builder.Services.AddDbContext<ReservationContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ReservationContext>();

// Ajouter des services au conteneur.
builder.Services.AddControllers().AddNewtonsoftJson();

// Configurer Swagger et l'authentification JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenJwtAuth();
builder.Services.AddCustomJwtAuth(builder.Configuration);
builder.Services.AddSwaggerGen();



// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var app = builder.Build();
app.UseCors("AllowAllOrigins");

// Initialiser les rôles et l'utilisateur admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await InitializeRoles(services);
}
// Configurer le pipeline de requêtes HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Projet de reservation de salles API V1");
    });
}

app.UseHttpsRedirection();
//
app.UseStaticFiles(); // Permet de servir les fichiers statiques

//
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
//
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//
app.Run();

// Méthode pour initialiser les rôles et les utilisateurs admin
async Task InitializeRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

    // Liste des rôles à créer
    string[] roleNames = { "Admin", "User" };
    IdentityResult roleResult;

    foreach (var roleName in roleNames)
    {
        // Vérifier si le rôle existe déjà
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            // Créer le rôle s'il n'existe pas
            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Créer un utilisateur admin s'il n'existe pas déjà
    var adminUser = await userManager.FindByEmailAsync("admin@example.com");
    if (adminUser == null)
    {
        adminUser = new AppUser()
        {
            UserName = "admin",
            Email = "admin@example.com"
        };
        await userManager.CreateAsync(adminUser, "Admin@123");

        // Ajouter le rôle Admin à l'utilisateur admin
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}