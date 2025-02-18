using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tesis.Data;
using Tesis.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("connectionString")
    ?? throw new InvalidOperationException("Connection string 'connectionString' not found.");

// Configurar ApplicationDbContext para Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configurar Identity con ApplicationDbContext
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddDbContext<DbtesisContext>(options =>
    options.UseSqlServer(connectionString));


// Agregar servicios para controladores y vistas
builder.Services.AddControllersWithViews();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();


//Crear roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Administrador", "Gestion" }; 

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

//Asociar Usuarios con los roles

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Asegúrate de que los roles están creados
    string[] roles = { "Administrador", "Gestion" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Busca al usuario en la tabla dbo.AspNetUsers
    var user = await userManager.FindByEmailAsync("santiperno@gmail.com"); 
    if (user != null)
    {
        // Asigna un rol al usuario
        if (!await userManager.IsInRoleAsync(user, "Administrador"))
        {
            await userManager.AddToRoleAsync(user, "Administrador");
        }
    }

    var user1 = await userManager.FindByEmailAsync("santi@gmail.com");
    if (user1 != null)
    {
        // Asigna un rol al usuario
        if (!await userManager.IsInRoleAsync(user1, "Gestion"))
        {
            await userManager.AddToRoleAsync(user1, "Gestion");
        }
    }
}


// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
