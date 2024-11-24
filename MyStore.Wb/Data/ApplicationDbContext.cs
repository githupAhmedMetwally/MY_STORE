using Microsoft.EntityFrameworkCore;
using MyStore.Wb.Models;

namespace MyStore.Wb
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
