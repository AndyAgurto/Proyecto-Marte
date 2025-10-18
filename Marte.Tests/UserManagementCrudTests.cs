using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Marte.Infrastructure.Data;
using Marte.Infrastructure.Repositories;
using Marte.Application.Services;
using Marte.Domain.Entities;

namespace Marte.Tests
{
    public class UserManagementCrudTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<MarteDbContext> _options;
        private readonly Guid _testRolId;

        public UserManagementCrudTests()
        {
            // In-memory SQLite connection
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<MarteDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Create schema and seed test data
            using var ctx = new MarteDbContext(_options);
            ctx.Database.EnsureCreated();

            // Create test role
            var testRole = new Rol
            {
                Id = Guid.NewGuid(),
                Nombre = "Usuario Estándar"
            };
            ctx.Roles.Add(testRole);
            ctx.SaveChanges();

            _testRolId = testRole.Id;
        }

        [Fact]
        public async Task Create_User_ShouldAddToDatabase()
        {
            // Arrange
            using var ctx = new MarteDbContext(_options);
            var usuarioRepo = new UsuarioRepository(ctx);
            var auditRepo = new AuditLogRepository(ctx);
            var authService = new AuthService(usuarioRepo, auditRepo);
            var userManagementService = new UserManagementService(usuarioRepo, auditRepo, authService);

            // Act
            var newUser = await userManagementService.CreateUserAsync(
                "testuser",
                "TestPassword123!",
                "Test User",
                _testRolId,
                "admin"
            );

            // Assert
            Assert.NotNull(newUser);
            Assert.Equal("testuser", newUser.NombreUsuario);
            Assert.Equal("Test User", newUser.NombreCompleto);
            Assert.True(newUser.Estado);

            // Verify user exists in database
            var userFromDb = await usuarioRepo.GetByIdAsync(newUser.Id);
            Assert.NotNull(userFromDb);
            Assert.Equal("testuser", userFromDb.NombreUsuario);
        }

        [Fact]
        public async Task Read_AllUsers_ShouldReturnUsers()
        {
            // Arrange
            using var ctx = new MarteDbContext(_options);
            var usuarioRepo = new UsuarioRepository(ctx);
            var auditRepo = new AuditLogRepository(ctx);
            var authService = new AuthService(usuarioRepo, auditRepo);
            var userManagementService = new UserManagementService(usuarioRepo, auditRepo, authService);

            // Create test users
            await userManagementService.CreateUserAsync("user1", "Pass1!", "User One", _testRolId, "admin");
            await userManagementService.CreateUserAsync("user2", "Pass2!", "User Two", _testRolId, "admin");

            // Act
            var users = await userManagementService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(users);
            Assert.True(users.Count() >= 2);
            Assert.Contains(users, u => u.NombreUsuario == "user1");
            Assert.Contains(users, u => u.NombreUsuario == "user2");
        }

        [Fact]
        public async Task Update_User_ShouldModifyUser()
        {
            // Arrange
            using var ctx = new MarteDbContext(_options);
            var usuarioRepo = new UsuarioRepository(ctx);
            var auditRepo = new AuditLogRepository(ctx);
            var authService = new AuthService(usuarioRepo, auditRepo);
            var userManagementService = new UserManagementService(usuarioRepo, auditRepo, authService);

            var user = await userManagementService.CreateUserAsync(
                "updatetest",
                "Password123!",
                "Original Name",
                _testRolId,
                "admin"
            );

            // Act
            await userManagementService.UpdateUserAsync(
                user.Id,
                "Updated Name",
                _testRolId,
                "admin"
            );

            // Assert
            var updatedUser = await usuarioRepo.GetByIdAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal("Updated Name", updatedUser.NombreCompleto);
        }

        [Fact]
        public async Task ChangePassword_ShouldUpdatePasswordHash()
        {
            // Arrange
            using var ctx = new MarteDbContext(_options);
            var usuarioRepo = new UsuarioRepository(ctx);
            var auditRepo = new AuditLogRepository(ctx);
            var authService = new AuthService(usuarioRepo, auditRepo);
            var userManagementService = new UserManagementService(usuarioRepo, auditRepo, authService);

            var user = await userManagementService.CreateUserAsync(
                "passtest",
                "OldPassword123!",
                "Password Test User",
                _testRolId,
                "admin"
            );

            var originalHash = user.ContraseñaHash;

            // Act
            await userManagementService.ChangePasswordAsync(user.Id, "NewPassword456!", "admin");

            // Assert
            var updatedUser = await usuarioRepo.GetByIdAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.NotEqual(originalHash, updatedUser.ContraseñaHash);
        }

        [Fact]
        public async Task DisableUser_ShouldSetEstadoToFalse()
        {
            // Arrange
            using var ctx = new MarteDbContext(_options);
            var usuarioRepo = new UsuarioRepository(ctx);
            var auditRepo = new AuditLogRepository(ctx);
            var authService = new AuthService(usuarioRepo, auditRepo);
            var userManagementService = new UserManagementService(usuarioRepo, auditRepo, authService);

            var user = await userManagementService.CreateUserAsync(
                "disabletest",
                "Password123!",
                "Disable Test User",
                _testRolId,
                "admin"
            );

            // Act
            await userManagementService.DisableUserAsync(user.Id, "admin");

            // Assert
            var disabledUser = await usuarioRepo.GetByIdAsync(user.Id);
            Assert.NotNull(disabledUser);
            Assert.False(disabledUser.Estado);
        }

        [Fact]
        public async Task EnableUser_ShouldSetEstadoToTrue()
        {
            // Arrange
            using var ctx = new MarteDbContext(_options);
            var usuarioRepo = new UsuarioRepository(ctx);
            var auditRepo = new AuditLogRepository(ctx);
            var authService = new AuthService(usuarioRepo, auditRepo);
            var userManagementService = new UserManagementService(usuarioRepo, auditRepo, authService);

            var user = await userManagementService.CreateUserAsync(
                "enabletest",
                "Password123!",
                "Enable Test User",
                _testRolId,
                "admin"
            );

            // Disable first
            await userManagementService.DisableUserAsync(user.Id, "admin");

            // Act
            await userManagementService.EnableUserAsync(user.Id, "admin");

            // Assert
            var enabledUser = await usuarioRepo.GetByIdAsync(user.Id);
            Assert.NotNull(enabledUser);
            Assert.True(enabledUser.Estado);
        }

        [Fact]
        public async Task UserExists_ShouldReturnTrueForExistingUser()
        {
            // Arrange
            using var ctx = new MarteDbContext(_options);
            var usuarioRepo = new UsuarioRepository(ctx);
            var auditRepo = new AuditLogRepository(ctx);
            var authService = new AuthService(usuarioRepo, auditRepo);
            var userManagementService = new UserManagementService(usuarioRepo, auditRepo, authService);

            await userManagementService.CreateUserAsync(
                "existstest",
                "Password123!",
                "Exists Test User",
                _testRolId,
                "admin"
            );

            // Act
            var exists = await userManagementService.UserExistsAsync("existstest");
            var notExists = await userManagementService.UserExistsAsync("nonexistent");

            // Assert
            Assert.True(exists);
            Assert.False(notExists);
        }

        [Fact]
        public async Task AuditLog_ShouldBeCreatedForUserOperations()
        {
            // Arrange
            using var ctx = new MarteDbContext(_options);
            var usuarioRepo = new UsuarioRepository(ctx);
            var auditRepo = new AuditLogRepository(ctx);
            var authService = new AuthService(usuarioRepo, auditRepo);
            var userManagementService = new UserManagementService(usuarioRepo, auditRepo, authService);

            // Act
            var user = await userManagementService.CreateUserAsync(
                "audittest",
                "Password123!",
                "Audit Test User",
                _testRolId,
                "admin"
            );

            // Assert - Verify audit log was created
            var auditLogs = await ctx.AuditLogs
                .Where(a => a.EntidadId == user.Id)
                .ToListAsync();

            Assert.NotEmpty(auditLogs);
            Assert.Contains(auditLogs, a => a.Accion == "Crear Usuario");
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
