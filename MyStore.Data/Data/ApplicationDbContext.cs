using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyStore.Models.Models;

namespace MyStore.Data
{
	public class ApplicationDbContext: IdentityDbContext<IdentityUser>
    {
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
: base(options)
		{
		}

        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
