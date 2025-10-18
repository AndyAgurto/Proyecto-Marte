using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Marte.Infrastructure.Data;
using Marte.Domain.Entities;
using Marte.Application.Services;
using Marte.Infrastructure.Repositories;

namespace Marte.Tests
{
    public class ConfiguracionTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<MarteDbContext> _options;

        public ConfiguracionTests()
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
        public async Task GetConfiguracionAsync_SinDatos_RetornaNull()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var configuracionRepo = new ConfiguracionEscuelaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new ConfiguracionService(configuracionRepo, auditRepo);

            // Act
            var result = await service.GetConfiguracionAsync();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetConfiguracionAsync_ConDatos_RetornaConfiguracion()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var configuracion = new ConfiguracionEscuela
            {
                Id = Guid.NewGuid(),
                NombreFilial = "Filial Test",
                HoraCierre = new TimeSpan(22, 0, 0)
            };
            await context.ConfiguracionesEscuela.AddAsync(configuracion);
            await context.SaveChangesAsync();

            var configuracionRepo = new ConfiguracionEscuelaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new ConfiguracionService(configuracionRepo, auditRepo);

            // Act
            var result = await service.GetConfiguracionAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Filial Test", result.NombreFilial);
            Assert.Equal(new TimeSpan(22, 0, 0), result.HoraCierre);
        }

        [Fact]
        public async Task UpdateHoraCierreAsync_ActualizaHora()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var configuracion = new ConfiguracionEscuela
            {
                Id = Guid.NewGuid(),
                NombreFilial = "Filial Original",
                HoraCierre = new TimeSpan(22, 0, 0)
            };
            await context.ConfiguracionesEscuela.AddAsync(configuracion);
            await context.SaveChangesAsync();

            var configuracionRepo = new ConfiguracionEscuelaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new ConfiguracionService(configuracionRepo, auditRepo);

            // Act
            var nuevaHora = new TimeSpan(23, 30, 0);
            var result = await service.UpdateHoraCierreAsync(nuevaHora, "Filial Actualizada");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nuevaHora, result.HoraCierre);
            Assert.Equal("Filial Actualizada", result.NombreFilial);
        }

        [Fact]
        public async Task UpdateHoraCierreAsync_SinConfiguracionExistente_CreaConfiguracion()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var configuracionRepo = new ConfiguracionEscuelaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new ConfiguracionService(configuracionRepo, auditRepo);

            // Act
            var nuevaHora = new TimeSpan(21, 0, 0);
            var result = await service.UpdateHoraCierreAsync(nuevaHora, "Nueva Filial");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nuevaHora, result.HoraCierre);
            Assert.Equal("Nueva Filial", result.NombreFilial);
        }

        [Fact]
        public async Task UpdateHoraCierreAsync_ValidaHoraValida()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var configuracion = new ConfiguracionEscuela
            {
                Id = Guid.NewGuid(),
                NombreFilial = "Filial Test",
                HoraCierre = new TimeSpan(22, 0, 0)
            };
            await context.ConfiguracionesEscuela.AddAsync(configuracion);
            await context.SaveChangesAsync();

            var configuracionRepo = new ConfiguracionEscuelaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new ConfiguracionService(configuracionRepo, auditRepo);

            // Act
            var horaValida = new TimeSpan(20, 30, 0); // 8:30 PM
            var result = await service.UpdateHoraCierreAsync(horaValida, "Filial Test");

            // Assert
            Assert.Equal(horaValida, result.HoraCierre);
        }

        [Fact]
        public async Task ConfiguracionEscuela_PropiedadesPersisten()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var configuracion = new ConfiguracionEscuela
            {
                Id = Guid.NewGuid(),
                NombreFilial = "Filial Completa",
                HoraCierre = new TimeSpan(22, 30, 0)
            };
            await context.ConfiguracionesEscuela.AddAsync(configuracion);
            await context.SaveChangesAsync();

            // Act - Leer desde nueva instancia del contexto
            using var context2 = new MarteDbContext(_options);
            var retrieved = await context2.ConfiguracionesEscuela.FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal("Filial Completa", retrieved.NombreFilial);
            Assert.Equal(new TimeSpan(22, 30, 0), retrieved.HoraCierre);
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
