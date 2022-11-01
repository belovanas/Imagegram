using System.Threading;
using System.Threading.Tasks;
using Domain;
using Dynamo.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Imagegram.Controllers
{
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        public Task Create([FromBody] string name, CancellationToken ct)
        {
            var user = new User()
            {
                Name = name
            };
            return _userRepository.Add(user, ct);
        }
    }
}