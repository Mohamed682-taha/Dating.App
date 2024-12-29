using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dating.Data.Entities;
using Dating.Data.IRepositories;
using Dating.Repository.Data;
using Dating.Shared;
using Microsoft.EntityFrameworkCore;

namespace Dating.Repository;

public class LikesRepository(DatingDbContext context, IMapper mapper) : ILikesRepository
{
    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PageList<MemberDto>> GetUserLikes(LikesParams likesParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDto> query;
        switch (likesParams.Predicate)
        {
            case "liked":
                query = likes
                    .Where(k => k.SourceUserId == likesParams.UserId)
                    .Select(k => k.TargetUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
            case "likedBy":
                query = likes
                    .Where(k => k.TargetUserId == likesParams.UserId)
                    .Select(k => k.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
            default:
                var likeIds = await GetCurrentUserLikeIds(likesParams.UserId);
                query = likes
                    .Where(k => k.TargetUserId == likesParams.UserId && likeIds.Contains(k.SourceUserId))
                    .Select(k => k.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
        }

        return await PageList<MemberDto>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
    }

    // users id that current user(lisa) liked
    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await context.Likes
            .Where(k => k.SourceUserId == currentUserId)
            .Select(k => k.TargetUserId)
            .ToListAsync();
    }

    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }
    
}