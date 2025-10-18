using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Marte.Infrastructure.Data;
using Marte.Domain.Entities;
using System.Linq;

namespace Marte.Tests
{
    public class DatabaseSeedTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<MarteDbContext> _options;

        public DatabaseSeedTests()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<MarteDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Create schema once
            using var context = new MarteDbContext(_options);
            context.Database.EnsureCreated();
        }

        [Fact]
        public async Task Verify_InitialData_IsCreatedCorrectly()
        {
            // Arrange
            using (var context = new MarteDbContext(_options))
            {
                // Add Roles
                var rolAdmin = new Rol 
                { 
                    Id = DatabaseSeeder.RolAdministradorId,
                    Nombre = "Administrador" 
                };
                var rolGuardia = new Rol 
                { 
                    Id = DatabaseSeeder.RolGuardiaId,
                    Nombre = "Guardia" 
                };
                await context.Roles.AddRangeAsync(rolAdmin, rolGuardia);

                // Add Admin User
                var admin = new Usuario
                {
                    Id = DatabaseSeeder.AdminUserId,
                    NombreUsuario = "druagurto",
                    NombreCompleto = "Andy Agurto Urcia",
                    ContraseñaHash = BCrypt.Net.BCrypt.HashPassword("@Druagurto00"),
                    RolId = DatabaseSeeder.RolAdministradorId,
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow,
                    Estado = true
                };
                await context.Usuarios.AddAsync(admin);

                // Add Categories
                var categorias = new[]
                {
                    "Estructural Jefe de Filial",
                    "Secretarios",
                    "GG.FF",
                    "GG.MM",
                    "Guardia de Seguridad",
                    "Miembros",
                    "Filosofía",
                    "Visitas",
                    "Otros"
                }.Select(nombre => new Categoria 
                { 
                    Id = Guid.NewGuid(),
                    Nombre = nombre
                });
                await context.Categorias.AddRangeAsync(categorias);

                await context.SaveChangesAsync();
            }

            // Act & Assert
            using (var context = new MarteDbContext(_options))
            {
                // Verify Roles
                var roles = await context.Roles.ToListAsync();
                Assert.Equal(2, roles.Count);
                Assert.Contains(roles, r => r.Id == DatabaseSeeder.RolAdministradorId && r.Nombre == "Administrador");
                Assert.Contains(roles, r => r.Id == DatabaseSeeder.RolGuardiaId && r.Nombre == "Guardia");

                // Verify Admin User
                var adminUser = await context.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.NombreUsuario == "druagurto");

                Assert.NotNull(adminUser);
                Assert.Equal("Andy Agurto Urcia", adminUser.NombreCompleto);
                Assert.Equal(DatabaseSeeder.RolAdministradorId, adminUser.RolId);
                Assert.Equal("Administrador", adminUser.Rol.Nombre);
                Assert.True(BCrypt.Net.BCrypt.Verify("@Druagurto00", adminUser.ContraseñaHash));

                // Verify Categories
                var categorias = await context.Categorias.ToListAsync();
                Assert.Equal(9, categorias.Count);
                Assert.Contains(categorias, c => c.Nombre == "Estructural Jefe de Filial");
                Assert.Contains(categorias, c => c.Nombre == "Secretarios");
                Assert.Contains(categorias, c => c.Nombre == "GG.FF");
                Assert.Contains(categorias, c => c.Nombre == "GG.MM");
                Assert.Contains(categorias, c => c.Nombre == "Guardia de Seguridad");
                Assert.Contains(categorias, c => c.Nombre == "Miembros");
                Assert.Contains(categorias, c => c.Nombre == "Filosofía");
                Assert.Contains(categorias, c => c.Nombre == "Visitas");
                Assert.Contains(categorias, c => c.Nombre == "Otros");
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}