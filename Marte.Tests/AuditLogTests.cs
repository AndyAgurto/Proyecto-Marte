using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Marte.Infrastructure.Data;
using Marte.Domain.Entities;
using Marte.Application.Services;
using Marte.Infrastructure.Repositories;
using System.IO;

namespace Marte.Tests
{
    public class AuditLogTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<MarteDbContext> _options;

        public AuditLogTests()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<MarteDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new MarteDbContext(_options);
            context.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetAllAsync_RetornaTodosLosLogs()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var logs = new[]
            {
                new AuditLog { Id = Guid.NewGuid(), Usuario = "user1", Accion = "LOGIN", Detalle = "Login exitoso", Fecha = DateTime.UtcNow },
                new AuditLog { Id = Guid.NewGuid(), Usuario = "user2", Accion = "CREAR_USUARIO", Detalle = "Usuario creado", Fecha = DateTime.UtcNow },
                new AuditLog { Id = Guid.NewGuid(), Usuario = "user1", Accion = "LOGOUT", Detalle = "Logout", Fecha = DateTime.UtcNow }
            };
            await context.AuditLogs.AddRangeAsync(logs);
            await context.SaveChangesAsync();

            var auditRepo = new AuditLogRepository(context);
            var service = new AuditLogService(auditRepo);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetByUsuarioAsync_FiltraPorUsuario()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var logs = new[]
            {
                new AuditLog { Id = Guid.NewGuid(), Usuario = "druagurto", Accion = "LOGIN", Detalle = "Login exitoso", Fecha = DateTime.UtcNow },
                new AuditLog { Id = Guid.NewGuid(), Usuario = "otrouser", Accion = "CREAR_USUARIO", Detalle = "Usuario creado", Fecha = DateTime.UtcNow },
                new AuditLog { Id = Guid.NewGuid(), Usuario = "druagurto", Accion = "LOGOUT", Detalle = "Logout", Fecha = DateTime.UtcNow }
            };
            await context.AuditLogs.AddRangeAsync(logs);
            await context.SaveChangesAsync();

            var auditRepo = new AuditLogRepository(context);
            var service = new AuditLogService(auditRepo);

            // Act
            var result = await service.GetByUsuarioAsync("druagurto");

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, log => Assert.Equal("druagurto", log.Usuario));
        }

        [Fact]
        public async Task GetByFechaRangoAsync_FiltraPorRangoFechas()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var hoy = DateTime.Today;
            var logs = new[]
            {
                new AuditLog { Id = Guid.NewGuid(), Usuario = "user1", Accion = "ACTION1", Detalle = "Detalle1", Fecha = hoy.AddDays(-5) },
                new AuditLog { Id = Guid.NewGuid(), Usuario = "user2", Accion = "ACTION2", Detalle = "Detalle2", Fecha = hoy.AddDays(-3) },
                new AuditLog { Id = Guid.NewGuid(), Usuario = "user3", Accion = "ACTION3", Detalle = "Detalle3", Fecha = hoy.AddDays(-1) },
                new AuditLog { Id = Guid.NewGuid(), Usuario = "user4", Accion = "ACTION4", Detalle = "Detalle4", Fecha = hoy }
            };
            await context.AuditLogs.AddRangeAsync(logs);
            await context.SaveChangesAsync();

            var auditRepo = new AuditLogRepository(context);
            var service = new AuditLogService(auditRepo);

            // Act
            var result = await service.GetByFechaRangoAsync(hoy.AddDays(-3), hoy);

            // Assert
            Assert.Equal(3, result.Count()); // Debe incluir -3, -1 y hoy
        }

        [Fact]
        public async Task GetByFiltrosAsync_ConUsuarioYFechas_FiltraCorrectamente()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var hoy = DateTime.Today;
            var logs = new[]
            {
                new AuditLog { Id = Guid.NewGuid(), Usuario = "druagurto", Accion = "ACTION1", Detalle = "Detalle1", Fecha = hoy.AddDays(-2) },
                new AuditLog { Id = Guid.NewGuid(), Usuario = "druagurto", Accion = "ACTION2", Detalle = "Detalle2", Fecha = hoy },
                new AuditLog { Id = Guid.NewGuid(), Usuario = "otrouser", Accion = "ACTION3", Detalle = "Detalle3", Fecha = hoy },
                new AuditLog { Id = Guid.NewGuid(), Usuario = "druagurto", Accion = "ACTION4", Detalle = "Detalle4", Fecha = hoy.AddDays(-10) }
            };
            await context.AuditLogs.AddRangeAsync(logs);
            await context.SaveChangesAsync();

            var auditRepo = new AuditLogRepository(context);
            var service = new AuditLogService(auditRepo);

            // Act
            var result = await service.GetByFiltrosAsync(hoy.AddDays(-5), hoy, "druagurto");

            // Assert
            Assert.Equal(2, result.Count()); // Solo druagurto en el rango -5 a hoy
            Assert.All(result, log => Assert.Equal("druagurto", log.Usuario));
        }

        [Fact]
        public async Task GetByFiltrosAsync_SoloFechas_FiltraSoloFechas()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var hoy = DateTime.Today;
            var logs = new[]
            {
                new AuditLog { Id = Guid.NewGuid(), Usuario = "user1", Accion = "ACTION1", Detalle = "Detalle1", Fecha = hoy.AddDays(-10) },
                new AuditLog { Id = Guid.NewGuid(), Usuario = "user2", Accion = "ACTION2", Detalle = "Detalle2", Fecha = hoy.AddDays(-1) },
                new AuditLog { Id = Guid.NewGuid(), Usuario = "user3", Accion = "ACTION3", Detalle = "Detalle3", Fecha = hoy }
            };
            await context.AuditLogs.AddRangeAsync(logs);
            await context.SaveChangesAsync();

            var auditRepo = new AuditLogRepository(context);
            var service = new AuditLogService(auditRepo);

            // Act
            var result = await service.GetByFiltrosAsync(hoy.AddDays(-2), hoy, null);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task ExportarATxtAsync_CreaArchivoCorrectamente()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var logs = new[]
            {
                new AuditLog { Id = Guid.NewGuid(), Usuario = "druagurto", Accion = "LOGIN", Detalle = "Login exitoso", Fecha = DateTime.UtcNow },
                new AuditLog { Id = Guid.NewGuid(), Usuario = "druagurto", Accion = "LOGOUT", Detalle = "Logout", Fecha = DateTime.UtcNow }
            };

            var auditRepo = new AuditLogRepository(context);
            var service = new AuditLogService(auditRepo);

            var tempFile = Path.Combine(Path.GetTempPath(), $"audit_test_{Guid.NewGuid()}.txt");

            try
            {
                // Act
                await service.ExportarATxtAsync(logs, tempFile);

                // Assert
                Assert.True(File.Exists(tempFile));
                var content = await File.ReadAllTextAsync(tempFile);
                Assert.Contains("druagurto", content);
                Assert.Contains("LOGIN", content);
            }
            finally
            {
                // Cleanup
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
