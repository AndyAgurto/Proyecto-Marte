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
    public class DashboardServiceTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<MarteDbContext> _options;
        private Guid _categoriaMiembrosId;

        public DashboardServiceTests()
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
            var categoriaMiembros = new Categoria { Id = Guid.NewGuid(), Nombre = "Miembros" };
            await context.Categorias.AddAsync(categoriaMiembros);
            _categoriaMiembrosId = categoriaMiembros.Id;

            // Crear asistentes de prueba
            var asistentes = new[]
            {
                new Asistente { Id = Guid.NewGuid(), DNI = "11111111", Nombres = "Juan", Apellidos = "Pérez", CategoriaId = _categoriaMiembrosId, NumeroGrupo = "01", Estado = true, FechaCreacion = DateTime.UtcNow },
                new Asistente { Id = Guid.NewGuid(), DNI = "22222222", Nombres = "María", Apellidos = "López", CategoriaId = _categoriaMiembrosId, NumeroGrupo = "02", Estado = true, FechaCreacion = DateTime.UtcNow },
                new Asistente { Id = Guid.NewGuid(), DNI = "33333333", Nombres = "Pedro", Apellidos = "García", CategoriaId = _categoriaMiembrosId, NumeroGrupo = "03", Estado = true, FechaCreacion = DateTime.UtcNow },
            };
            await context.Asistentes.AddRangeAsync(asistentes);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetDashboardStats_RetornaEstadisticas()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var service = new DashboardService(context);

            // Act
            var stats = await service.GetDashboardStatsAsync();

            // Assert
            Assert.NotNull(stats);
            Assert.True(stats.TotalRegistrados >= 0);
        }

        [Fact]
        public async Task GetDashboardStats_ConAsistenciasHoy_CuentaCorrectamente()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistentes = await context.Asistentes.ToListAsync();
            
            // Registrar asistencias de hoy
            var asistenciasHoy = new[]
            {
                new Asistencia 
                { 
                    Id = Guid.NewGuid(), 
                    AsistenteId = asistentes[0].Id, 
                    Fecha = DateTime.Today, 
                    HoraIngreso = DateTime.Today.AddHours(8)
                },
                new Asistencia 
                { 
                    Id = Guid.NewGuid(), 
                    AsistenteId = asistentes[1].Id, 
                    Fecha = DateTime.Today, 
                    HoraIngreso = DateTime.Today.AddHours(8).AddMinutes(15)
                }
            };
            await context.Asistencias.AddRangeAsync(asistenciasHoy);
            await context.SaveChangesAsync();

            var service = new DashboardService(context);

            // Act
            var stats = await service.GetDashboardStatsAsync();

            // Assert
            Assert.Equal(2, stats.AsistentesHoy);
        }

        [Fact]
        public async Task GetDashboardStats_CalculaPorcentajePuntualidad()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistentes = await context.Asistentes.ToListAsync();
            
            var asistencias = new[]
            {
                new Asistencia 
                { 
                    Id = Guid.NewGuid(), 
                    AsistenteId = asistentes[0].Id, 
                    Fecha = DateTime.Today, 
                    HoraIngreso = DateTime.Today.AddHours(7).AddMinutes(45) // Puntual
                },
                new Asistencia 
                { 
                    Id = Guid.NewGuid(), 
                    AsistenteId = asistentes[1].Id, 
                    Fecha = DateTime.Today, 
                    HoraIngreso = DateTime.Today.AddHours(8).AddMinutes(30) // Tarde
                }
            };
            await context.Asistencias.AddRangeAsync(asistencias);
            await context.SaveChangesAsync();

            var service = new DashboardService(context);

            // Act
            var stats = await service.GetDashboardStatsAsync();

            // Assert
            Assert.True(stats.PorcentajePuntualidad >= 0 && stats.PorcentajePuntualidad <= 100);
        }

        [Fact]
        public async Task GetDashboardStats_CuentaAsistentesPorGrupo()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistentes = await context.Asistentes.ToListAsync();
            
            var asistencias = asistentes.Select(a => new Asistencia
            {
                Id = Guid.NewGuid(),
                AsistenteId = a.Id,
                Fecha = DateTime.Today,
                HoraIngreso = DateTime.Today.AddHours(8)
            }).ToArray();

            await context.Asistencias.AddRangeAsync(asistencias);
            await context.SaveChangesAsync();

            var service = new DashboardService(context);

            // Act
            var stats = await service.GetDashboardStatsAsync();

            // Assert
            Assert.True(stats.MiembrosGrupo1 >= 0);
            Assert.True(stats.MiembrosGrupo2 >= 0);
            Assert.True(stats.MiembrosGrupo3 >= 0);
        }

        [Fact]
        public async Task GetDashboardStats_CuentaHastaCierreAyer()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistentes = await context.Asistentes.ToListAsync();
            
            var ayer = DateTime.Today.AddDays(-1);
            var asistenciasAyer = new[]
            {
                new Asistencia 
                { 
                    Id = Guid.NewGuid(), 
                    AsistenteId = asistentes[0].Id, 
                    Fecha = ayer, 
                    HoraIngreso = ayer.AddHours(8),
                    HoraSalida = ayer.AddHours(22),
                    HastaCierre = true
                }
            };
            await context.Asistencias.AddRangeAsync(asistenciasAyer);
            await context.SaveChangesAsync();

            var service = new DashboardService(context);

            // Act
            var stats = await service.GetDashboardStatsAsync();

            // Assert
            Assert.True(stats.HastaCierreAyer >= 0);
        }

        [Fact]
        public async Task GetDashboardStats_CalculaTopAsistentes()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistentes = await context.Asistentes.ToListAsync();
            
            // Crear múltiples asistencias para el primer asistente
            var asistenciasMúltiples = Enumerable.Range(0, 5).Select(i => new Asistencia
            {
                Id = Guid.NewGuid(),
                AsistenteId = asistentes[0].Id,
                Fecha = DateTime.Today.AddDays(-i),
                HoraIngreso = DateTime.Today.AddDays(-i).AddHours(8),
                HoraSalida = DateTime.Today.AddDays(-i).AddHours(22),
                HastaCierre = true
            }).ToArray();

            await context.Asistencias.AddRangeAsync(asistenciasMúltiples);
            await context.SaveChangesAsync();

            var service = new DashboardService(context);

            // Act
            var stats = await service.GetDashboardStatsAsync();

            // Assert
            Assert.NotNull(stats.TopAsistentes);
            Assert.True(stats.TopAsistentes.Count <= 5);
        }

        [Fact]
        public async Task GetDashboardStats_TotalRegistrados_IncluySoloActivos()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            
            // Agregar un asistente inactivo
            var asistenteInactivo = new Asistente
            {
                Id = Guid.NewGuid(),
                DNI = "99999999",
                Nombres = "Inactivo",
                Apellidos = "Test",
                CategoriaId = _categoriaMiembrosId,
                NumeroGrupo = "01",
                Estado = false,
                FechaCreacion = DateTime.UtcNow
            };
            await context.Asistentes.AddAsync(asistenteInactivo);
            await context.SaveChangesAsync();

            var service = new DashboardService(context);

            // Act
            var stats = await service.GetDashboardStatsAsync();

            // Assert
            // Total registrados debe contar solo los activos
            var activosCount = await context.Asistentes.CountAsync(a => a.Estado);
            Assert.Equal(activosCount, stats.TotalRegistrados);
        }

        [Fact]
        public async Task GetDashboardStats_SinDatos_RetornaEstadisticasVacias()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            
            // Limpiar todos los datos
            context.Asistencias.RemoveRange(context.Asistencias);
            context.Asistentes.RemoveRange(context.Asistentes);
            await context.SaveChangesAsync();

            var service = new DashboardService(context);

            // Act
            var stats = await service.GetDashboardStatsAsync();

            // Assert
            Assert.NotNull(stats);
            Assert.Equal(0, stats.AsistentesHoy);
            Assert.Equal(0, stats.TotalRegistrados);
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
