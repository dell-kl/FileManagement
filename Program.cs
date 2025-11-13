using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SIS_DIAF.Caching;
using SIS_DIAF.DTO;
using SIS_DIAF.Models;
using SIS_DIAF.Repositorios;
using SIS_DIAF.Security;
using SIS_DIAF.Seed;
using SIS_DIAF.Services;
using SIS_DIAF.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddServerSideBlazor();

builder.Services.AddDbContext<SistemaDiafContext>((serviceProvider, option) =>
{
    option.UseSqlServer(
        builder.Configuration.GetConnectionString("CadenaConexion")
     );
});

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<SistemaDiafContext>()
    .SetApplicationName("SistemaDiaf");

builder.Services.AddScoped<IRepositorio<Usuario>, RUsuario>();
builder.Services.AddScoped<IRepositorio<Rol>, RRol>(); 
builder.Services.AddScoped<ILogin, RUsuario>();
builder.Services.AddScoped<ICorreoRepository, RCorreo>();
builder.Services.AddScoped<IEmailService, EmailServicio>();
builder.Services.AddScoped<IResponsableRegimen, RResponsableRegimen>();

//para la parte de la proteccion de los datos.
builder.Services.AddDataProtection();

//vamos a configurar nuestra memoria cache compartida de segundo nivel
//para que todos los procesos lo puedan usar y no tengan que hacer consultas nuevamente directo a la DB.
//revisar carpeta -> /Caching/CacheCompartida.cs <-
//builder.Services.AddMemoryCache(); 

//configuracion nuestra autenticacion y autorizacion.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(
    option =>
    {
        option.LoginPath = "/Formulario/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        option.AccessDeniedPath = "/AccesoUsuario/Denegado";
        option.Cookie.Name = "CookieSesion";
    }
);



builder.Services.AddScoped<RResponsableRegimen>();
builder.Services.AddScoped<RUsuario>();
builder.Services.AddScoped<UploadFiles>();
builder.Services.AddScoped<RArchivo>();
builder.Services.AddScoped<RHistorico>();
builder.Services.AddScoped<RTipoArchivo>();
builder.Services.AddScoped<RRegimen>();
builder.Services.AddScoped<RSucuarsales>();
builder.Services.AddSingleton<EncriptacionPass>();
builder.Services.AddScoped<GestionarEncriptado>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ISesion, SesionService>();
builder.Services.AddTransient<SesionService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SistemaDiafContext>();
        var scopedEncriptacion = services.GetRequiredService<EncriptacionPass>();

        SeederDiaf l = new SeederDiaf(scopedEncriptacion);
        l.SeederDBDiaf(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

//app.MapBlazorHub();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
