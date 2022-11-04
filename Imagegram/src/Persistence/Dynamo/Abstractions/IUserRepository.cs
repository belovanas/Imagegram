using System.Threading;
using System.Threading.Tasks;

namespace Dynamo.Abstractions
{
    public interface IUserRepository
    {
        Task Add(string login, CancellationToken ct);
        Task<bool> DoesExist(string login, CancellationToken ct);
    }
}