using Dating.Data.Entities;

namespace Dating.Data.IServices;

public interface ITokenService
{
    string CreateToken(AppUser user);
}