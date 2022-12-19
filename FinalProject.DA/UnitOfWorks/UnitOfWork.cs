using WebApplication1.DataAccess.Repositories.ArticleRepository;
using WebApplication1.DataAccess.Repositories.CommentRepository;
using WebApplication1.DataAccess.Repositories.FavoriteRepository;
using WebApplication1.DataAccess.Repositories.FollowRepository;
using WebApplication1.DataAccess.Repositories.RefreshTokenRepository;
using WebApplication1.DataAccess.Repositories.UsersRepository;

namespace WebApplication1.DataAccess.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProjectDbContext _projectDbContext;
        public UnitOfWork(ProjectDbContext projectDbContext, IFollowRepository follows, IArticleRepository articles
            , ICommentsRepository comments, IUsersRepository users, IRefreshTokenRepository refreshTokens
            , IFavouriteRepository favorites)
        {
            _projectDbContext = projectDbContext;
            Follows = follows;
            Articles = articles;
            Comments = comments;
            Users = users;
            RefreshTokens = refreshTokens;
            Favorites = favorites;
        }
        public IFollowRepository Follows { get; private set; }
        public IArticleRepository Articles { get; private set; }
        public ICommentsRepository Comments { get; private set; }
        public IUsersRepository Users { get; private set; }
        public IRefreshTokenRepository RefreshTokens { get; private set; }
        public IFavouriteRepository Favorites { get; private set; }
        public async Task<int> complete()
        {
            return await _projectDbContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _projectDbContext.DisposeAsync();
        }
    }
}
