using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Domain.Models.DTO_s.ResponseDto_s;
using Domain.Models.DTO_s.RequestDto_s;
using DataAcess.UnitOfWorks;
using Domain.Exceptions;
using DataAcess.Entites;
using Domain.Services.SessionService;

namespace Domain.Services.CommentService
{
    public class CommentsService : ICommentsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ActionResult> PostComment(int articleId, UserSessionModel user, CommentPostRequestDto request)
        {
            if (user is null)
                throw new UnauthorizedUserException("you need to re-login");

            if (!await _unitOfWork.Articles.DoesExistAsync(a => a.ArticleId == articleId))
                throw new RecordNotFoundException("specified article cannot be found");

            var comment = new Comments
            {
                CommentContent = request.CommentContent,
                ArticleId = articleId,
                UserId = user.UserId
            };

            _unitOfWork.Comments.AddAsync(comment);
            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }
        public async Task<CommentResponseDto> GetCommentOnArticle(int articleId, int commentId)
        {
            if (!await _unitOfWork.Articles.DoesExistAsync(A => A.ArticleId == articleId))
                throw new RecordNotFoundException("specified article cannot be found");

            if (!await _unitOfWork.Comments.DoesExistAsync(C => C.CommentId == commentId))
                throw new RecordNotFoundException("specified comment cannot be found");

            var comment = await _unitOfWork.Comments.GetAsync(commentId);
            var user = await _unitOfWork.Users.GetAsync(comment.UserId);
            var response = _mapper.Map<CommentResponseDto>((comment, user));

            return await Task.FromResult(response);
        }

        public async Task<IEnumerable<CommentResponseDto>> GetCommentsOnArticle(int articleId)
        {
            if (!await _unitOfWork.Articles.DoesExistAsync(a => a.ArticleId == articleId))
                throw new RecordNotFoundException("specified article cannot be found");

            var comments = await _unitOfWork.Comments.FindAsync(c => c.ArticleId == articleId);

            if (comments.Count() == 0)
                return Enumerable.Empty<CommentResponseDto>();

            List<CommentResponseDto> responses = new List<CommentResponseDto>(); // should use select 
            foreach (var comment in comments)
            {
                var user = await _unitOfWork.Users.GetAsync(comment.UserId);
                var commentResponse = _mapper.Map<CommentResponseDto>((comment, user));
                responses.Add(commentResponse);
            }

            return await Task.FromResult(responses);
        }
        public async Task<ActionResult> DeleteCommentOnArticle(int articleId, UserSessionModel user, int commentId)
        {
            if (user is null)
                throw new UnauthorizedUserException("you need to re-login");

            if (!await _unitOfWork.Articles.DoesExistAsync(a => a.ArticleId == articleId))
                throw new RecordNotFoundException("there is no article with the specified id");

            if (!await _unitOfWork.Comments.DoesExistAsync(c => c.CommentId == commentId))
                throw new RecordNotFoundException("there is no comment with the specified id");

            var comment = await _unitOfWork.Comments.GetAsync(commentId);

            if (user.UserId != comment.UserId)
                throw new UnauthorizedUserException("you cant delete another user comment");

            _unitOfWork.Comments.Remove(comment);
            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }

    }
}
