using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Dynamo.Abstractions
{
    public interface IUserRepository
    {
        Task Add(User user, CancellationToken ct);
        Task<User> GetByName(string name, CancellationToken ct);
    }
}