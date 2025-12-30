using Microsoft.AspNetCore.Identity;
using System.Data;

namespace CologneStore.Data
{
	public class DbSeeder
	{
		public static async Task SeedDefaultData(IServiceProvider service)
		{
			var userManager = service.GetService<UserManager<IdentityUser>>();
			var roleManager = service.GetService<RoleManager<IdentityRole>>();
			//Adding some roles to db 


			await roleManager.CreateAsync(new IdentityRole("Admin"));
			await roleManager.CreateAsync(new IdentityRole("User"));

			//Create admin user

			var admin = new IdentityUser
			{
				UserName = "admin@gmail.com",
				Email = "admin@gmail.com",
				EmailConfirmed = true
			};

			var userInDb = await userManager.FindByEmailAsync(admin.Email);
			if (userInDb == null)
			{
				await userManager.CreateAsync(admin, "Admin@123");
				await userManager.AddToRoleAsync(admin, "Admin");
			}
		}
	}
}
