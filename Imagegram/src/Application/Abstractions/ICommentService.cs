using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Application.Abstractions
{
    public interface ICommentService
    {
        Task Add(Comment comment, CancellationToken ct);
        Task Delete(string commentId, string postId, string user, CancellationToken ct);
    }
}