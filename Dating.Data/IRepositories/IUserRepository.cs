using Dating.Data.Entities;
using Dating.Shared;

namespace Dating.Data.IRepositories;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserAsync(int id);
    Task<AppUser?> GetUserByUserName(string username);
    Task<bool> SaveAllChangesAsync();
    Task<PageList<MemberDto>> GetMembersAsync(UserParams userParams);
    Task<MemberDto?> GetMemberByUserNameAsync(string username);
}