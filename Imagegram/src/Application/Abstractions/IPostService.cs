using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Application.Abstractions
{
    public interface IPostService
    {
        Task Create(Post post, Stream image, CancellationToken ct);
        Task Delete(string id, CancellationToken ct);
        Task<Post> Get(string id, CancellationToken ct);
        Task<List<Post>> GetPage(int limit, int offset, CancellationToken ct);
    }
}