var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registro de Repositorios y Servicios
builder.Services.AddScoped<PruebaTécnicaWebConLogin.Repositories.UsuarioRepository>();
builder.Services.AddSingleton<PruebaTécnicaWebConLogin.Services.PasswordHasher>();

// ===================================================================
// FALTA ESTO: El servicio de sesiones debe estar registrado en el contenedor
// ===================================================================
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Tiempo por defecto en servidor
    options.Cookie.HttpOnly = true;                 // Mitigación OWASP contra XSS
    options.Cookie.IsEssential = true;              // Esencial para el funcionamiento del sistema
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 1. Primero se habilitan las rutas
app.UseRouting();

// ===================================================================
// ORDEN CORRECTO: La sesión debe prepararse ANTES de la autorización
// ===================================================================
app.UseSession();

// 3. Luego se procesa la autorización (que ya puede disponer de los datos de la sesión si fuese necesario)
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Bienvenida}/{id?}")
    .WithStaticAssets();

app.Run();