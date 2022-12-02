
using WebApplication1.DataAccess;
using WebApplication1.DataAccess.Repositories.ArticleRepository;
using WebApplication1.DataAccess.Repositories.CommentRepository;
using WebApplication1.DataAccess.Repositories.FavoriteRepository;
using WebApplication1.DataAccess.Repositories.FollowRepository;
using WebApplication1.DataAccess.Repositories.RefreshTokenRepository;
using WebApplication1.DataAccess.Repositories.UsersRepository;

namespace WebApplication1.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProjectDbContext _projectDbContext;
        public UnitOfWork(ProjectDbContext projectDbContext) // injection via c
        {
            _projectDbContext = projectDbContext;
            Follows = new FollowRepository(_projectDbContext);
            Articles = new ArticleRepository(_projectDbContext);
            Comments = new CommentsRepository(_projectDbContext);
            Users = new UsersRepository(_projectDbContext);
            RefreshTokens = new RefreshTokensRepository(_projectDbContext);
        }
        public   IFollowRepository Follows { get; private set; }
        public IArticleRepository Articles { get; private set; }
        public ICommentsRepository Comments { get; private set; }
        public IUsersRepository Users { get; private set; }
        public IRefreshTokenRepository RefreshTokens { get; private set; }
        public IFavouriteRepository Favorites { get; private set; }
        public int complete()
        {
            return _projectDbContext.SaveChanges();
        }
        public void Dispose()
        {
            _projectDbContext.Dispose();
        }
    }
}
