using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Dating.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Dating.Repository.Data;

public static class DatingDbContextSeed
{
    public static async Task DataSeed(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (!userManager.Users.Any())
        {
            var usersData = await File.ReadAllTextAsync("../Dating.Repository/Data/DataSeed/UserSeedData.json");
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<AppUser>>(usersData, options);
            if (users?.Count > 0)
            {
                var roles = new List<AppRole>()
                {
                    new() { Name = "Member" },
                    new() { Name = "Admin" },
                    new() { Name = "Moderator" }
                };
                foreach (var role in roles) await roleManager.CreateAsync(role);
                foreach (var user in users)
                {
                    user.UserName = user.UserName!.ToLower();
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                    await userManager.AddToRoleAsync(user, "Member");
                }

                var admin = new AppUser()
                {
                    UserName = "Admin",
                    KnownAs = "Admin",
                    City = "",
                    Country = "",
                    Gender = ""
                };
                await userManager.CreateAsync(admin, "Pa$$w0rd");
                await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);
            }
        }
    }
}