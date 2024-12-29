using Dating.Data.IRepositories;
using Dating.Repository.Data;

namespace Dating.Repository;

public class UnitOfWork(
    IUserRepository userRepoo,
    IMessageRepository messageRepo,
    ILikesRepository likesRepo,
    DatingDbContext context
) : IUnitOfWork
{
    public IUserRepository UserRepository { get; } = userRepoo;
    public IMessageRepository MessageRepository { get; } = messageRepo;
    public ILikesRepository LikesRepository { get; } = likesRepo;

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}