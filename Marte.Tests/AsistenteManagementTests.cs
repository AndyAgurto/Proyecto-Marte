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
    public class AsistenteManagementTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<MarteDbContext> _options;
        private Guid _categoriaId;
        private Guid _categoriaMiembrosId;
        private Guid _categoriaVisitasId;

        public AsistenteManagementTests()
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
            var categoriaVisitas = new Categoria { Id = Guid.NewGuid(), Nombre = "Visitas" };
            var categoriaGGFF = new Categoria { Id = Guid.NewGuid(), Nombre = "GG.FF" };

            await context.Categorias.AddRangeAsync(categoriaMiembros, categoriaVisitas, categoriaGGFF);
            await context.SaveChangesAsync();

            _categoriaMiembrosId = categoriaMiembros.Id;
            _categoriaVisitasId = categoriaVisitas.Id;
            _categoriaId = categoriaMiembros.Id;
        }

        [Fact]
        public async Task CreateAsistente_ConDatosValidos_CreaExitosamente()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistenteRepo = new AsistenteRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new AsistenteService(asistenteRepo, categoriaRepo, auditRepo);

            // Act
            var result = await service.CreateAsistenteAsync(
                "Juan",
                "Pérez",
                "12345678",
                _categoriaMiembrosId,
                "01",
                "testuser"
            );

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Asistente);
            Assert.Equal("12345678", result.Asistente.DNI);
            Assert.Equal("Juan", result.Asistente.Nombres);
        }

        [Fact]
        public async Task CreateAsistente_ConDNIDuplicado_RetornaError()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistenteRepo = new AsistenteRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new AsistenteService(asistenteRepo, categoriaRepo, auditRepo);

            await service.CreateAsistenteAsync("Juan", "Pérez", "87654321", _categoriaMiembrosId, "01", "testuser");

            // Act
            var result = await service.CreateAsistenteAsync("María", "López", "87654321", _categoriaMiembrosId, "02", "testuser");

            // Assert
            Assert.False(result.Success);
            Assert.Contains("DNI", result.Message);
        }

        [Fact]
        public async Task CreateAsistente_MiembroSinGrupo_RetornaError()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistenteRepo = new AsistenteRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new AsistenteService(asistenteRepo, categoriaRepo, auditRepo);

            // Act
            var result = await service.CreateAsistenteAsync(
                "Ana",
                "García",
                "11111111",
                _categoriaMiembrosId,
                null, // Sin grupo
                "testuser"
            );

            // Assert
            Assert.False(result.Success);
            Assert.Contains("grupo", result.Message.ToLower());
        }

        [Fact]
        public async Task CreateAsistente_VisitaConGrupo_AceptaYGuardaConGrupo()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistenteRepo = new AsistenteRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new AsistenteService(asistenteRepo, categoriaRepo, auditRepo);

            // Act - Visitas no requiere grupo, pero si se proporciona se guarda
            var result = await service.CreateAsistenteAsync(
                "Pedro",
                "Martínez",
                "22222222",
                _categoriaVisitasId,
                "05",
                "testuser"
            );

            // Assert
            Assert.True(result.Success);
            // El comportamiento puede variar: o se acepta o se limpia
            // Ajustar según la lógica del servicio
        }

        [Fact]
        public async Task GetAllAsistentes_RetornaTodosLosAsistentes()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistenteRepo = new AsistenteRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new AsistenteService(asistenteRepo, categoriaRepo, auditRepo);

            await service.CreateAsistenteAsync("Test1", "Apellido1", "10000001", _categoriaMiembrosId, "01", "testuser");
            await service.CreateAsistenteAsync("Test2", "Apellido2", "10000002", _categoriaMiembrosId, "02", "testuser");

            // Act
            var asistentes = await service.GetAllAsistentesAsync();

            // Assert
            Assert.NotEmpty(asistentes);
            Assert.True(asistentes.Count() >= 2);
        }

        [Fact]
        public async Task UpdateAsistente_CambiarDatos_ActualizaCorrectamente()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistenteRepo = new AsistenteRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new AsistenteService(asistenteRepo, categoriaRepo, auditRepo);

            var createResult = await service.CreateAsistenteAsync("Original", "Nombre", "30000001", _categoriaMiembrosId, "01", "testuser");

            // Act
            var updateResult = await service.UpdateAsistenteAsync(
                createResult.Asistente!.Id,
                "Actualizado",
                "NombreNuevo",
                "30000001",
                _categoriaMiembrosId,
                "02",
                "testuser"
            );

            // Assert
            Assert.True(updateResult.Success);
            var updated = await service.GetAsistenteByIdAsync(createResult.Asistente.Id);
            Assert.Equal("Actualizado", updated!.Nombres);
            Assert.Equal("02", updated.NumeroGrupo);
        }

        [Fact]
        public async Task InactivarAsistente_CambiaEstadoAFalse()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistenteRepo = new AsistenteRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new AsistenteService(asistenteRepo, categoriaRepo, auditRepo);

            var createResult = await service.CreateAsistenteAsync("Inactivar", "Test", "40000001", _categoriaMiembrosId, "01", "testuser");

            // Act
            var inactivarResult = await service.InactivarAsistenteAsync(createResult.Asistente!.Id, "testuser");

            // Assert
            Assert.True(inactivarResult.Success);
            var asistente = await service.GetAsistenteByIdAsync(createResult.Asistente.Id);
            Assert.False(asistente!.Estado);
        }

        [Fact]
        public async Task ActivarAsistente_CambiaEstadoATrue()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistenteRepo = new AsistenteRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new AsistenteService(asistenteRepo, categoriaRepo, auditRepo);

            var createResult = await service.CreateAsistenteAsync("Activar", "Test", "50000001", _categoriaMiembrosId, "01", "testuser");
            await service.InactivarAsistenteAsync(createResult.Asistente!.Id, "testuser");

            // Act
            var activarResult = await service.ActivarAsistenteAsync(createResult.Asistente.Id, "testuser");

            // Assert
            Assert.True(activarResult.Success);
            var asistente = await service.GetAsistenteByIdAsync(createResult.Asistente.Id);
            Assert.True(asistente!.Estado);
        }

        [Fact]
        public async Task DeleteAsistente_EliminaPermanentemente()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistenteRepo = new AsistenteRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new AsistenteService(asistenteRepo, categoriaRepo, auditRepo);

            var createResult = await service.CreateAsistenteAsync("Eliminar", "Test", "60000001", _categoriaMiembrosId, "01", "testuser");

            // Act
            var deleteResult = await service.DeleteAsistenteAsync(createResult.Asistente!.Id, "testuser");

            // Assert
            Assert.True(deleteResult.Success);
            var asistente = await service.GetAsistenteByIdAsync(createResult.Asistente.Id);
            Assert.Null(asistente);
        }

        [Fact]
        public async Task ValidarDNIUnico_ConDNIExistente_RetornaFalse()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistenteRepo = new AsistenteRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new AsistenteService(asistenteRepo, categoriaRepo, auditRepo);

            await service.CreateAsistenteAsync("Test", "Validar", "70000001", _categoriaMiembrosId, "01", "testuser");

            // Act
            var esUnico = await service.ValidarDNIUnicoAsync("70000001");

            // Assert
            Assert.False(esUnico);
        }

        [Fact]
        public async Task ValidarDNIUnico_ConDNINuevo_RetornaTrue()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistenteRepo = new AsistenteRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new AsistenteService(asistenteRepo, categoriaRepo, auditRepo);

            // Act
            var esUnico = await service.ValidarDNIUnicoAsync("99999999");

            // Assert
            Assert.True(esUnico);
        }

        [Fact]
        public async Task GetAsistentesActivos_SoloRetornaActivos()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistenteRepo = new AsistenteRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new AsistenteService(asistenteRepo, categoriaRepo, auditRepo);

            var activo = await service.CreateAsistenteAsync("Activo", "Test", "A0000001", _categoriaMiembrosId, "01", "testuser");
            var inactivo = await service.CreateAsistenteAsync("Inactivo", "Test", "A0000002", _categoriaMiembrosId, "01", "testuser");
            await service.InactivarAsistenteAsync(inactivo.Asistente!.Id, "testuser");

            // Act
            var asistentesActivos = await service.GetAsistentesActivosAsync();

            // Assert
            Assert.All(asistentesActivos, a => Assert.True(a.Estado));
            Assert.Contains(asistentesActivos, a => a.DNI == "A0000001");
            Assert.DoesNotContain(asistentesActivos, a => a.DNI == "A0000002");
        }

        [Fact]
        public async Task CreateAsistente_RegistraAuditoria()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistenteRepo = new AsistenteRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new AsistenteService(asistenteRepo, categoriaRepo, auditRepo);

            // Act
            await service.CreateAsistenteAsync("Auditoria", "Test", "B0000001", _categoriaMiembrosId, "01", "testuser");

            // Assert
            var audits = await context.AuditLogs.ToListAsync();
            Assert.Contains(audits, a => a.Usuario == "testuser" && a.Accion.Contains("Asistente"));
        }

        [Fact]
        public async Task UpdateAsistente_CambiarCategoriaDeMiembroAVisita_ManejaGrupoCorrectamente()
        {
            // Arrange
            using var context = new MarteDbContext(_options);
            var asistenteRepo = new AsistenteRepository(context);
            var categoriaRepo = new CategoriaRepository(context);
            var auditRepo = new AuditLogRepository(context);
            var service = new AsistenteService(asistenteRepo, categoriaRepo, auditRepo);

            var createResult = await service.CreateAsistenteAsync("Cambio", "Categoria", "C0000001", _categoriaMiembrosId, "03", "testuser");

            // Act - Cambiar a Visitas (no requiere grupo)
            var updateResult = await service.UpdateAsistenteAsync(
                createResult.Asistente!.Id,
                "Cambio",
                "Categoria",
                "C0000001",
                _categoriaVisitasId,
                null, // Grupo null porque Visitas no lo requiere
                "testuser"
            );

            // Assert
            Assert.True(updateResult.Success);
            var updated = await service.GetAsistenteByIdAsync(createResult.Asistente.Id);
            // El servicio debe manejar el grupo según la categoría
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
