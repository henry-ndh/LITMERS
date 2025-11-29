using Microsoft.EntityFrameworkCore;

namespace TFU.Logger
{
	public class TFULoggerContext : DbContext
	{
		public TFULoggerContext(string sqliteConnectionString) : base(GetOptions(sqliteConnectionString))
		{
		}

		private static DbContextOptions GetOptions(string sqliteConnectionString)
		{
			return SqliteDbContextOptionsBuilderExtensions.UseSqlite(new DbContextOptionsBuilder(), sqliteConnectionString).Options;
		}

		public DbSet<TFULoggerModel> Logger { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<TFULoggerModel>(s =>
			{
				s.HasKey(k => k.Id);
				s.ToTable("TFU_Logger");
			});
		}
	}
}
