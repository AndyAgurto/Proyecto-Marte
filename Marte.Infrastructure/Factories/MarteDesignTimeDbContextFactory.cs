using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Marte.Infrastructure.Data;

namespace Marte.Infrastructure.Factories
{
	/// <summary>
	/// Factory para crear el DbContext en tiempo de diseño (usado por EF Core Tools para migraciones)
	/// </summary>
	public class MarteDesignTimeDbContextFactory : IDesignTimeDbContextFactory<MarteDbContext>
	{
		public MarteDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<MarteDbContext>();

			// ⭐ IMPORTANTE: En desarrollo seguir usando DRUAGURTO para crear/aplicar migraciones
			// En producción (Release), la app usará LocalDB automáticamente
			#if DEBUG
			// Desarrollo: Usar instancia DRUAGURTO
			optionsBuilder.UseSqlServer(@"Server=.\DRUAGURTO;Database=MarteDb;Trusted_Connection=True;TrustServerCertificate=True;");
			#else
			// Producción: Usar LocalDB (para migraciones en build Release si es necesario)
			optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MarteDb;Trusted_Connection=True;");
			#endif

			return new MarteDbContext(optionsBuilder.Options);
		}
	}
}

