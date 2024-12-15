using Dating.Data.Entities;
using Dating.Data.IRepositories;
using Dating.Repository.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dating.Shared;

namespace Dating.Repository;

public class UserRepository(DatingDbContext context, IMapper mapper) : IUserRepository
{
    public void Update(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await context.Users.Include(u => u.Photos).AsNoTracking().ToListAsync();
    }

    public async Task<AppUser?> GetUserAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetUserByUserName(string username)
    {
        return await context.Users.Include(u => u.Photos).SingleOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<bool> SaveAllChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<PageList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var query = context.Users.ProjectTo<MemberDto>(mapper.ConfigurationProvider);
        return await PageList<MemberDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
    }

    public async Task<MemberDto?> GetMemberByUserNameAsync(string username)
    {
        var user = await context.Users.Where(u => u.UserName == username)
            .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        return user;
    }
}