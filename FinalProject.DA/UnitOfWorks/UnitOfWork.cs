using DataAcess.Repositories.ArticleRepository;
using DataAcess.Repositories.CommentRepository;
using DataAcess.Repositories.FavoriteRepository;
using DataAcess.Repositories.FollowRepository;
using DataAcess.Repositories.RefreshTokenRepository;
using DataAcess.Repositories.UsersRepository;

namespace DataAcess.UnitOfWorks
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
