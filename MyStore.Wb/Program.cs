using Microsoft.EntityFrameworkCore;
using MyStore.Data;
using MyStore.Data.Impelementaion;
using MyStore.Models.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MyStore.Utilities;
using Stripe;

namespace MyStore.Wb
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            //builder.Services.AddRazorPages();
            builder.Services.AddDbContext<ApplicationDbContext>(
	 options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
			builder.Services.Configure<StripeData>(builder.Configuration.GetSection("Stripe"));
   builder.Services.AddIdentity<IdentityUser,IdentityRole>
				(option=>option.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromHours(4))
				.AddDefaultTokenProviders().AddDefaultUI()
				.AddEntityFrameworkStores<ApplicationDbContext>();
			
            
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			builder.Services.AddSingleton<IEmailSender, EmailSender>();
            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
			}
			app.UseStaticFiles();

			app.UseRouting();
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:Secretkey").Get<string>();
            app.UseAuthorization();
            app.MapRazorPages();
            app.MapControllerRoute(
				name: "default",
				pattern: "{area=Admin}/{controller=Category}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "Customer",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
		}
	}
}
