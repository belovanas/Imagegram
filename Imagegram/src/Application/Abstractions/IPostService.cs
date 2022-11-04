using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Application.Abstractions
{
    public interface IPostService
    {
        Task Save(Post post, Stream image, CancellationToken ct);
        Task<Post> GetById(string id, CancellationToken ct);
        Task<Stream> GetImageForPost(string id, CancellationToken ct);
    }
}