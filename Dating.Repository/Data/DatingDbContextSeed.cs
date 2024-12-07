using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Dating.Data.Entities;

namespace Dating.Repository.Data;

public static class DatingDbContextSeed
{
    public static async Task DataSeed(DatingDbContext context)
    {
        if (!context.Users.Any())
        {
            var usersData = await File.ReadAllTextAsync("../Dating.Repository/Data/DataSeed/UserSeedData.json");
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<AppUser>>(usersData, options);
            if (users?.Count > 0)
            {
                foreach (var user in users)
                {
                    using var hmac = new HMACSHA512();
                    user.UserName = user.UserName.ToLower();
                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                    user.PasswordSalt = hmac.Key;
                    context.Users.Add(user);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}