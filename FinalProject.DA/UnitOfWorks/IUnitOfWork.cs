using DataAcess.Repositories.ArticleRepository;
using DataAcess.Repositories.CommentRepository;
using DataAcess.Repositories.FavoriteRepository;
using DataAcess.Repositories.FollowRepository;
using DataAcess.Repositories.RefreshTokenRepository;
using DataAcess.Repositories.UsersRepository;

namespace DataAcess.UnitOfWorks
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IFollowRepository Follows { get; }
        IArticleRepository Articles { get; }
        ICommentsRepository Comments { get; }
        IUsersRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IFavouriteRepository Favorites { get; }
        Task<int> complete();
    }
}
