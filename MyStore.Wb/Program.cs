using Microsoft.EntityFrameworkCore;
using MyStore.Data;
using MyStore.Data.Impelementaion;
using MyStore.Models.Repositories;

namespace MyStore.Wb
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();
			builder.Services.AddDbContext<ApplicationDbContext>(
	 options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
			builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
			}
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{area=Admin}/{controller=Category}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
