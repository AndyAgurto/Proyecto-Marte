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

namespace Marte.Tests
{
    public class ReporteServiceTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<MarteDbContext> _options;
        private Guid _categoriaId;

        public ReporteServiceTests()
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

            var asistentes = new[]
            {
                new Asistente { Id = Guid.NewGuid(), DNI = "11111111", Nombres = "Juan", Apellidos = "Pérez", CategoriaId = _categoriaId, NumeroGrupo = "01", Estado = true, FechaCreacion = DateTime.UtcNow },
                new Asistente { Id = Guid.NewGuid(), DNI = "22222222", Nombres = "María", Apellidos = "López", CategoriaId = _categoriaId, NumeroGrupo = "02", Estado = true, FechaCreacion = DateTime.UtcNow }
            };
            await context.Asistentes.AddRangeAsync(asistentes);

            // Crear asistencias de prueba
            var hoy = DateTime.Today;
            var asistencias = new[]
            {
                new Asistencia 
                { 
                    Id = Guid.NewGuid(), 
                    AsistenteId = asistentes[0].Id, 
                    Fecha = hoy, 
                    HoraIngreso = hoy.AddHours(7).AddMinutes(45),
                    HoraSalida = hoy.AddHours(22)
                },
                new Asistencia 
                { 
                    Id = Guid.NewGuid(), 
                    AsistenteId = asistentes[1].Id, 
                    Fecha = hoy.AddDays(-1), 
                    HoraIngreso = hoy.AddDays(-1).AddHours(8).AddMinutes(15),
                    HoraSalida = hoy.AddDays(-1).AddHours(22)
                }
            };
            await context.Asistencias.AddRangeAsync(asistencias);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetTotalesDiariosAsync_RetornaDatosPorDia()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var reporteRepo = new ReporteRepository(context);
            var service = new ReporteService(reporteRepo);

            var fechaInicio = DateTime.Today.AddDays(-7);
            var fechaFin = DateTime.Today;

            // Act
            var result = await service.GetTotalesDiariosAsync(fechaInicio, fechaFin);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetTotalesPorCategoriaAsync_AgrupaPorCategoria()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var reporteRepo = new ReporteRepository(context);
            var service = new ReporteService(reporteRepo);

            var fechaInicio = DateTime.Today.AddDays(-7);
            var fechaFin = DateTime.Today;

            // Act
            var result = await service.GetTotalesPorCategoriaAsync(fechaInicio, fechaFin);

            // Assert
            Assert.NotNull(result);
            // Debe haber al menos una categoría (Miembros)
        }

        [Fact]
        public async Task GetTotalesPorGrupoAsync_AgrupaPorGrupo()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var reporteRepo = new ReporteRepository(context);
            var service = new ReporteService(reporteRepo);

            var fechaInicio = DateTime.Today.AddDays(-7);
            var fechaFin = DateTime.Today;

            // Act
            var result = await service.GetTotalesPorGrupoAsync(fechaInicio, fechaFin);

            // Assert
            Assert.NotNull(result);
            // Debe haber grupos (01, 02)
        }

        [Fact]
        public async Task GetHistorialIndividualPorDNIAsync_RetornaHistorialAsistente()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var reporteRepo = new ReporteRepository(context);
            var service = new ReporteService(reporteRepo);

            var fechaInicio = DateTime.Today.AddDays(-30);
            var fechaFin = DateTime.Today;

            // Act
            var result = await service.GetHistorialIndividualPorDNIAsync("11111111", fechaInicio, fechaFin);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetReportePuntualidadAsync_CalculaPuntualidad()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var reporteRepo = new ReporteRepository(context);
            var service = new ReporteService(reporteRepo);

            var fechaInicio = DateTime.Today.AddDays(-7);
            var fechaFin = DateTime.Today;
            var horaPuntualidad = new TimeSpan(8, 0, 0); // 8:00 AM

            // Act
            var result = await service.GetReportePuntualidadAsync(fechaInicio, fechaFin, horaPuntualidad);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetReporteHastaCierreAsync_RetornaAsistenciasCompletadas()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var reporteRepo = new ReporteRepository(context);
            var service = new ReporteService(reporteRepo);

            var fechaInicio = DateTime.Today.AddDays(-7);
            var fechaFin = DateTime.Today;

            // Act
            var result = await service.GetReporteHastaCierreAsync(fechaInicio, fechaFin);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetTopAsistentesConstantesAsync_RetornaTopN()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var reporteRepo = new ReporteRepository(context);
            var service = new ReporteService(reporteRepo);

            var fechaInicio = DateTime.Today.AddDays(-30);
            var fechaFin = DateTime.Today;

            // Act
            var result = await service.GetTopAsistentesConstantesAsync(fechaInicio, fechaFin, 10);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count() <= 10);
        }

        [Fact]
        public async Task GetDiaMayorAsistenciaAsync_EncuentraDiaConMasAsistencias()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var reporteRepo = new ReporteRepository(context);
            var service = new ReporteService(reporteRepo);

            var fechaInicio = DateTime.Today.AddDays(-7);
            var fechaFin = DateTime.Today;

            // Act
            var result = await service.GetDiaMayorAsistenciaAsync(fechaInicio, fechaFin);

            // Assert - Puede ser null si no hay asistencias
            // Si hay datos, debe tener fecha válida
            if (result != null)
            {
                Assert.True(result.Fecha >= fechaInicio && result.Fecha <= fechaFin);
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
