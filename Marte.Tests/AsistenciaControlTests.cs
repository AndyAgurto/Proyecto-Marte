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

// Helper method to create AsistenciaService with all dependencies
namespace Marte.Tests
{
    internal static class TestHelpers
    {
        public static AsistenciaService CreateAsistenciaService(MarteDbContext context)
        {
            var asistenciaRepo = new AsistenciaRepository(context);
            var asistenteRepo = new AsistenteRepository(context);
            var configuracionRepo = new ConfiguracionEscuelaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            return new AsistenciaService(asistenciaRepo, asistenteRepo, auditRepo, configuracionRepo, categoriaRepo);
        }
    }
}

namespace Marte.Tests
{
    public class AsistenciaControlTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<MarteDbContext> _options;
        private Asistente _asistenteTest;
        private Guid _categoriaId;
        private Guid _configuracionId;

        public AsistenciaControlTests()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<MarteDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new MarteDbContext(_options);
            context.Database.EnsureCreated();
            SeedData(context).Wait();
        }

        private async Task SeedData(MarteDbContext context)
        {
            var categoria = new Categoria { Id = Guid.NewGuid(), Nombre = "Miembros" };
            await context.Categorias.AddAsync(categoria);
            _categoriaId = categoria.Id;

            var configuracion = new ConfiguracionEscuela
            {
                Id = Guid.NewGuid(),
                NombreFilial = "Filial Test",
                HoraCierre = new TimeSpan(22, 0, 0)
            };
            await context.ConfiguracionesEscuela.AddAsync(configuracion);
            _configuracionId = configuracion.Id;

            _asistenteTest = new Asistente
            {
                Id = Guid.NewGuid(),
                DNI = "12345678",
                Nombres = "Juan",
                Apellidos = "Pérez",
                CategoriaId = _categoriaId,
                NumeroGrupo = "01",
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };
            await context.Asistentes.AddAsync(_asistenteTest);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task RegistrarIngreso_ConDNIValido_CreaAsistencia()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var service = TestHelpers.CreateAsistenciaService(context);

            // Act
            var result = await service.RegistrarIngresoAsync(_asistenteTest.DNI, "testuser");

            // Assert
            Assert.True(result.Success);
            var asistencias = await context.Asistencias.ToListAsync();
            Assert.Single(asistencias);
            Assert.Equal(_asistenteTest.Id, asistencias[0].AsistenteId);
        }

        [Fact]
        public async Task RegistrarIngreso_ConDNINoExistente_RetornaError()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var service = TestHelpers.CreateAsistenciaService(context);

            // Act
            var result = await service.RegistrarIngresoAsync("99999999", "testuser");

            // Assert
            Assert.False(result.Success);
            Assert.Contains("no existe", result.Message.ToLower());
        }

        [Fact]
        public async Task RegistrarIngreso_YaRegistradoHoy_RetornaError()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var service = TestHelpers.CreateAsistenciaService(context);

            await service.RegistrarIngresoAsync(_asistenteTest.DNI, "testuser");

            // Act - Intentar registrar de nuevo
            var result = await service.RegistrarIngresoAsync(_asistenteTest.DNI, "testuser");

            // Assert
            Assert.False(result.Success);
            Assert.Contains("ya registr", result.Message.ToLower());
        }

        [Fact]
        public async Task RegistrarSalida_ConEntradaPrevia_ActualizaHoraSalida()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var service = TestHelpers.CreateAsistenciaService(context);

            await service.RegistrarIngresoAsync(_asistenteTest.DNI, "testuser");
            var asistencias = await context.Asistencias.ToListAsync();
            var asistenciaId = asistencias[0].Id;

            // Act
            var result = await service.RegistrarSalidaAsync(asistenciaId, "testuser");

            // Assert
            Assert.True(result.Success);
            var asistencia = await context.Asistencias.FindAsync(asistenciaId);
            Assert.NotNull(asistencia!.HoraSalida);
        }

        [Fact]
        public async Task RegistrarSalida_ConObservacion_GuardaObservacion()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var service = TestHelpers.CreateAsistenciaService(context);

            await service.RegistrarIngresoAsync(_asistenteTest.DNI, "testuser");
            var asistencias = await context.Asistencias.ToListAsync();
            var asistenciaId = asistencias[0].Id;

            // Act
            var result = await service.RegistrarSalidaAsync(asistenciaId, "testuser", "Salida anticipada");

            // Assert
            Assert.True(result.Success);
            var asistencia = await context.Asistencias.FindAsync(asistenciaId);
            Assert.Equal("Salida anticipada", asistencia!.Observacion);
        }

        [Fact]
        public async Task AplicarCierreAutomatico_CierraTodosLosAbiertos()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var service = TestHelpers.CreateAsistenciaService(context);

            // Crear 2 asistentes más
            var asistente2 = new Asistente
            {
                Id = Guid.NewGuid(),
                DNI = "87654321",
                Nombres = "María",
                Apellidos = "López",
                CategoriaId = _categoriaId,
                NumeroGrupo = "02",
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };
            await context.Asistentes.AddAsync(asistente2);
            await context.SaveChangesAsync();

            await service.RegistrarIngresoAsync(_asistenteTest.DNI, "testuser");
            await service.RegistrarIngresoAsync(asistente2.DNI, "testuser");

            // Act
            var result = await service.AplicarCierreAutomaticoAsync("testuser");

            // Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.Actualizados);
            var asistencias = await context.Asistencias.ToListAsync();
            Assert.All(asistencias, a => Assert.NotNull(a.HoraSalida));
        }

        [Fact]
        public async Task GetAsistenciasPresentesAsync_RetornaSoloSinSalida()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var service = TestHelpers.CreateAsistenciaService(context);

            await service.RegistrarIngresoAsync(_asistenteTest.DNI, "testuser");

            // Act
            var presentes = await service.GetAsistenciasPresentesAsync();

            // Assert
            Assert.Single(presentes);
            Assert.All(presentes, a => Assert.Null(a.HoraSalida));
        }

        [Fact]
        public async Task GetAsistenciasByFecha_RetornaAsistenciasDelDia()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var service = TestHelpers.CreateAsistenciaService(context);

            await service.RegistrarIngresoAsync(_asistenteTest.DNI, "testuser");
            var hoy = DateTime.Today;

            // Act
            var asistencias = await service.GetAsistenciasByFechaAsync(hoy);

            // Assert
            Assert.NotEmpty(asistencias);
            Assert.All(asistencias, a => Assert.Equal(hoy, a.Fecha.Date));
        }

        [Fact]
        public async Task RegistrarIngreso_RegistraAuditoria()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var service = TestHelpers.CreateAsistenciaService(context);

            // Act
            await service.RegistrarIngresoAsync(_asistenteTest.DNI, "testuser");

            // Assert
            var audits = await context.AuditLogs.ToListAsync();
            Assert.Contains(audits, a => a.Usuario == "testuser" && a.Accion.Contains("Ingreso"));
        }

        [Fact]
        public async Task GetAllAsistencias_RetornaTodasLasAsistencias()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var service = TestHelpers.CreateAsistenciaService(context);

            await service.RegistrarIngresoAsync(_asistenteTest.DNI, "testuser");

            // Act
            var todas = await service.GetAllAsistenciasAsync();

            // Assert
            Assert.NotEmpty(todas);
        }

        [Fact]
        public async Task RegistrarIngreso_AsistenteInactivo_RetornaError()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var service = TestHelpers.CreateAsistenciaService(context);

            // Inactivar asistente
            _asistenteTest.Estado = false;
            context.Asistentes.Update(_asistenteTest);
            await context.SaveChangesAsync();

            // Act
            var result = await service.RegistrarIngresoAsync(_asistenteTest.DNI, "testuser");

            // Assert
            Assert.False(result.Success);
            Assert.Contains("inactiv", result.Message.ToLower());
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
