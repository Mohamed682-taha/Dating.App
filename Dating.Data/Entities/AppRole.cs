using Microsoft.AspNetCore.Identity;

namespace Dating.Data.Entities;

public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; } = [];
}