using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Dynamo.Abstractions
{
    public interface IPostRepository
    {
        Task Add(Post postInfo, CancellationToken ct);
        Task Delete(string id, CancellationToken ct);
        Task<Post> Get(string id, CancellationToken ct);
        Task<List<Post>> GetAll(CancellationToken ct);
    }
}