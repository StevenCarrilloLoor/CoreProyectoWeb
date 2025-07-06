using Microsoft.EntityFrameworkCore;
using PetrolriosFraudeDetection.Data;
using PetrolriosFraudeDetection.Interfaces;
using PetrolriosFraudeDetection.Models.Services;
using PetrolriosFraudeDetection.Services;
using PetrolriosFraudeDetection.Validators;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro de servicios con las mejoras implementadas
builder.Services.AddScoped<CodigoEstacionValidator>();
builder.Services.AddScoped<NumeroFacturaValidator>();

// MEJORA: Registrar la interfaz IMotorDeteccion con su implementación mejorada
// Esto implementa el Principio de Inversión de Dependencias (DIP)
builder.Services.AddScoped<IMotorDeteccion, MotorDeteccionMejorado>();

// Mantener el servicio original para compatibilidad si es necesario
builder.Services.AddScoped<MotorDeteccion>();

// Configuración de MVC 
builder.Services.AddControllersWithViews();

// Agregar servicios de API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // URL de la app React
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

var app = builder.Build();

// Configure el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Usar CORS
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapear controladores API
app.MapControllers();

app.Run();