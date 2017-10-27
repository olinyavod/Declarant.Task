using System.Data.Entity;
using Declarant.Task.Models;
using SQLite.CodeFirst;

namespace Declarant.Task
{
    public class DeclarantContext : DbContext
    {
	    public DeclarantContext()
	    {
			Configuration.LazyLoadingEnabled = false;
		    Configuration.ProxyCreationEnabled = false;
		}

	    protected override void OnModelCreating(DbModelBuilder modelBuilder)
	    {
		    base.OnModelCreating(modelBuilder);

		    modelBuilder.Entity<EventItem>()
			    .HasKey(m => m.Id)
			    .ToTable("EventItems");

			modelBuilder.Entity<EventItem>()
				.Property(m => m.Name)
			    .HasMaxLength(128)
			    .IsVariableLength()
				.IsUnicode();

		    modelBuilder.Entity<EventItem>()
			    .Property(m => m.Desciption)
			    .HasMaxLength(1024)
			    .IsVariableLength()
			    .IsUnicode();

		    Database.SetInitializer(new SqliteDropCreateDatabaseWhenModelChanges<DeclarantContext>(modelBuilder));

		}
	}
}
