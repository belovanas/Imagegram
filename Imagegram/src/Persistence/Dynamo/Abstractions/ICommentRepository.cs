using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Dynamo.Abstractions
{
    public interface ICommentRepository
    {
        Task Add(Comment comment, CancellationToken ct);
        Task Delete(string id, string postId, CancellationToken ct);
        Task<List<Comment>> GetByPostId(string postId, CancellationToken ct);
    }
}