using Microsoft.EntityFrameworkCore;
using MyStore.Models.Models;

namespace MyStore.Data
{
	public class ApplicationDbContext:DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
: base(options)
		{
		}

        public DbSet<Category> Category { get; set; }
    }
}
