using AutoMapper;
using FinalProject.DL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.DataAccess.UnitOfWorks;
using WebApplication1.Models.Requests;
using WebApplication1.Models;
using WebApplication1.Models.Response;
using WebApplication1.Models.Entites;

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
        public async Task<ActionResult> PostComment(int articleId, UserSessionModel user, CommentRequest request)
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
        public async Task<CommentResponse> GetCommentOnArticle(int articleId, int commentId)
        {
            if (!await _unitOfWork.Articles.DoesExistAsync(A => A.ArticleId == articleId))
                throw new RecordNotFoundException("specified article cannot be found");

            if (!await _unitOfWork.Comments.DoesExistAsync(C => C.CommentId == commentId))
                throw new RecordNotFoundException("specified comment cannot be found");

            var comment = await _unitOfWork.Comments.GetAsync(commentId);
            var user = await _unitOfWork.Users.GetAsync(comment.UserId);
            var response = _mapper.Map<CommentResponse>((comment, user));

            return await Task.FromResult(response);
        }

        public async Task<IEnumerable<CommentResponse>> GetCommentsOnArticle(int articleId)
        {
            if (!await _unitOfWork.Articles.DoesExistAsync(a => a.ArticleId == articleId))
                throw new RecordNotFoundException("specified article cannot be found");


            var comments = await _unitOfWork.Comments.FindAsync(c => c.ArticleId == articleId);
            if (comments.Count() == 0)
                return Enumerable.Empty<CommentResponse>();


            List<CommentResponse> responses = new List<CommentResponse>();
            foreach (var comment in comments)
            {
                var user = await _unitOfWork.Users.GetAsync(comment.UserId);
                var commentResponse = _mapper.Map<CommentResponse>((comment, user));
                responses.Add(commentResponse);
            }

            return await Task.FromResult(responses);
        }
        public async Task<ActionResult> DeleteCommentOnArticle(int articleId, UserSessionModel user, CommentDeleteRequest request)
        {
            if (user is null)
                throw new UnauthorizedUserException("you need to re-login");

            if (!await _unitOfWork.Articles.DoesExistAsync(a => a.ArticleId == articleId))
                throw new RecordNotFoundException("there is no article with the specified id");

            if (!await _unitOfWork.Comments.DoesExistAsync(c => c.CommentId == request.commentId))
                throw new RecordNotFoundException("there is no comment with the specified id");

            var comment = await _unitOfWork.Comments.GetAsync(request.commentId);

            if (user.UserId != comment.UserId)
                throw new UnauthorizedUserException("you cant delete another user comment");

            _unitOfWork.Comments.Remove(comment);
            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }

    }
}
