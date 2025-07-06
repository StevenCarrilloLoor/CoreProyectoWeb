using Microsoft.EntityFrameworkCore;
using PetrolriosFraudeDetection.Models.Entities;

namespace PetrolriosFraudeDetection.Data
{
    public class ApplicationDbContext : DbContext
    {
        

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        // Constructor sin parámetros para Entity Framework
        public ApplicationDbContext()
        {
        }
        
        // Configuración de la conexión en caso de no recibir opciones
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=STEVEN-ALIENWAR\\SQLTRABAJO;Database=PetrolriosFraudeDetection;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
        //public DbSet<Ubicacion> Ubicaciones {get; set;}
        public DbSet<Estacion> Estaciones { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<SensorVolumen> SensoresVolumen { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<AlertaFraude> AlertasFraude { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           
            // Configuraciones y restricciones
            modelBuilder.Entity<Estacion>()
                .HasIndex(e => e.Codigo)
                .IsUnique();
               
            modelBuilder.Entity<Factura>()
                .HasIndex(f => f.NumeroFactura)
                .IsUnique();
               
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
               
            // Datos de prueba para demostración
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario {
                    Id = 1,
                    Nombre = "Administrador",
                    Apellido = "Sistema",
                    Email = "admin@petrolrios.com",
                    PasswordHash = "hashed_password",
                    Rol = "Administrador"
                }
            );
           
            modelBuilder.Entity<Estacion>().HasData(
                new Estacion {
                    Id = 1,
                    Nombre = "Estación Central",
                    Ubicacion = "Quito Norte",
                    Codigo = "EST-000001",
                    Activo = true
                },
                new Estacion {
                    Id = 2,
                    Nombre = "Estación Sur",
                    Ubicacion = "Quito Sur",
                    Codigo = "EST-000002",
                    Activo = true
                }
            );
        }
    }
}