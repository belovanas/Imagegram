using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain;
using Dynamo.Abstractions;

namespace Application
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task Add(Comment comment, CancellationToken ct)
        {
            await _commentRepository.Add(comment, ct);
        }

        public async Task Delete(string commentId, string postId, string user, CancellationToken ct)
        {
            var commentModel = await _commentRepository.Get(commentId, postId, ct);
            if (commentModel.User != user)
            {
                throw new ApplicationException("Can't delete other people's comments");
            }

            await _commentRepository.Delete(commentId, postId, ct);
        }
    }
}