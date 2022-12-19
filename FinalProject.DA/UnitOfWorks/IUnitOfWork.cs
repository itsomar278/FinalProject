using WebApplication1.DataAccess.Repositories.ArticleRepository;
using WebApplication1.DataAccess.Repositories.CommentRepository;
using WebApplication1.DataAccess.Repositories.FavoriteRepository;
using WebApplication1.DataAccess.Repositories.FollowRepository;
using WebApplication1.DataAccess.Repositories.RefreshTokenRepository;
using WebApplication1.DataAccess.Repositories.UsersRepository;

namespace WebApplication1.DataAccess.UnitOfWorks
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
