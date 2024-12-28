
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using projet1.Data;
using projet1.Extention;
using projet1.models;

var builder = WebApplication.CreateBuilder(args);

//
builder.Services.AddControllersWithViews();
//
// Configurer le contexte de la base de donn�es
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

// Initialiser les r�les et l'utilisateur admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await InitializeRoles(services);
}
// Configurer le pipeline de requ�tes HTTP.
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

// M�thode pour initialiser les r�les et les utilisateurs admin
async Task InitializeRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

    // Liste des r�les � cr�er
    string[] roleNames = { "Admin", "User" };
    IdentityResult roleResult;

    foreach (var roleName in roleNames)
    {
        // V�rifier si le r�le existe d�j�
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            // Cr�er le r�le s'il n'existe pas
            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Cr�er un utilisateur admin s'il n'existe pas d�j�
    var adminUser = await userManager.FindByEmailAsync("admin@example.com");
    if (adminUser == null)
    {
        adminUser = new AppUser()
        {
            UserName = "admin",
            Email = "admin@example.com"
        };
        await userManager.CreateAsync(adminUser, "Admin@123");

        // Ajouter le r�le Admin � l'utilisateur admin
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}