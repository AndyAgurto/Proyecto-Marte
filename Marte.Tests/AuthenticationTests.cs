using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Marte.Infrastructure.Data;
using Marte.Domain.Entities;
using BCrypt.Net;
using Marte.Application.Services;

namespace Marte.Tests
{
    public class AuthenticationTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<MarteDbContext> _options;

        public AuthenticationTests()
        {
            // In-memory SQLite connection, persists for the life of the connection
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<MarteDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Create schema
            using var ctx = new MarteDbContext(_options);
            ctx.Database.EnsureCreated();
        }

        [Fact]
        public async Task AdminUser_PasswordHash_VerifiesAndSessionSignInWorks()
        {
            // Arrange: seed a minimal set (role + admin user)
            var adminUser = new Usuario
            {
                Id = Guid.NewGuid(),
                NombreUsuario = "druagurto",
                NombreCompleto = "Andy Agurto Urcia",
                ContraseñaHash = BCrypt.Net.BCrypt.HashPassword("@Druagurto00"),
                FechaCreacion = DateTime.UtcNow,
                FechaModificacion = DateTime.UtcNow,
                Estado = true
            };

            using (var ctx = new MarteDbContext(_options))
            {
                var rol = new Rol { Id = Guid.NewGuid(), Nombre = "Administrador" };
                ctx.Roles.Add(rol);
                // link admin to role
                adminUser.RolId = rol.Id;
                ctx.Usuarios.Add(adminUser);
                await ctx.SaveChangesAsync();
            }

            // Act & Assert: verify password hash
            using (var ctx = new MarteDbContext(_options))
            {
                var userFromDb = await ctx.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == "druagurto");
                Assert.NotNull(userFromDb);
                Assert.True(BCrypt.Net.BCrypt.Verify("@Druagurto00", userFromDb.ContraseñaHash));
            }

            // SessionService sign in / sign out
            var session = new SessionService();
            Assert.False(session.IsAuthenticated);
            await session.SignInAsync(adminUser);
            Assert.True(session.IsAuthenticated);
            Assert.NotNull(session.CurrentUser);
            Assert.Equal("druagurto", session.CurrentUser.NombreUsuario);

            await session.SignOutAsync();
            Assert.False(session.IsAuthenticated);
            Assert.Null(session.CurrentUser);
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}