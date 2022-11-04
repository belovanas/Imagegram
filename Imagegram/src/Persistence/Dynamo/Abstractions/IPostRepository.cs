using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Dynamo.Abstractions
{
    public interface IPostRepository
    {
        Task Add(PostInfo postInfo, CancellationToken ct);
        Task Delete(string id, CancellationToken ct);
        Task<PostInfo> Get(string id, CancellationToken ct);
    }
}