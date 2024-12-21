using Dating.Data.Entities;

namespace Dating.Data.IServices;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
}