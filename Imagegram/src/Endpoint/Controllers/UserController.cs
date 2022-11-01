using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Dynamo.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imagegram.Controllers
{
    [Authorize]
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
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = new User()
            {
                Name = name
            };
            return _userRepository.Add(user, ct);
        }
    }
}