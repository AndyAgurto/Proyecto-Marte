using System;
using System.Threading.Tasks;
using Marte.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Marte.SeedRunner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("   DATABASE SEEDER - MARTE");
            Console.WriteLine("=================================================\n");

            var services = new ServiceCollection();
            
            // Configurar DbContext
            services.AddDbContext<MarteDbContext>(options =>
                options.UseSqlServer(@"Server=.\DRUAGURTO;Database=MarteDb;Trusted_Connection=True;TrustServerCertificate=True;"));

            var serviceProvider = services.BuildServiceProvider();

            try
            {
                Console.WriteLine("Aplicando migraciones...");
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MarteDbContext>();
                    await dbContext.Database.MigrateAsync();
                }
                Console.WriteLine("✅ Migraciones aplicadas correctamente\n");

                Console.WriteLine("Ejecutando DatabaseSeeder...");
                await DatabaseSeeder.SeedAsync(serviceProvider);
                Console.WriteLine("✅ DatabaseSeeder ejecutado correctamente\n");

                // Verificar datos insertados
                Console.WriteLine("Verificando datos insertados:");
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MarteDbContext>();
                    
                    var rolesCount = await dbContext.Roles.CountAsync();
                    var usuariosCount = await dbContext.Usuarios.CountAsync();
                    var categoriasCount = await dbContext.Categorias.CountAsync();

                    Console.WriteLine($"  - Roles: {rolesCount}");
                    Console.WriteLine($"  - Usuarios: {usuariosCount}");
                    Console.WriteLine($"  - Categorías: {categoriasCount}");
                }

                Console.WriteLine("\n=================================================");
                Console.WriteLine("   SEEDING COMPLETADO EXITOSAMENTE");
                Console.WriteLine("=================================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ ERROR: {ex.Message}");
                Console.WriteLine($"Detalles: {ex.InnerException?.Message}");
                Console.WriteLine($"\nStack Trace:\n{ex.StackTrace}");
                return;
            }
        }
    }
}
