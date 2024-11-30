using Dating.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dating.Repository.Data;

public class DatingDbContext(DbContextOptions<DatingDbContext>options):DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
}