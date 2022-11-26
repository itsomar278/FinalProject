using WebApplication1.Models.Repositories.ArticleRepository;
using WebApplication1.Models.Repositories.CommentsRepository;
using WebApplication1.Models.Repositories.FollowRepository;
using WebApplication1.Models.Repositories.UsersRepository;

namespace WebApplication1.Models.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IFollowRepository Follows {get;}
        IArticleRepository Articles { get;}
        ICommentsRepository Comments { get;}
        IUsersRepository Users { get;}
        int complete();
    }
}
