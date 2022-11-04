using System.Threading;
using System.Threading.Tasks;
using Dynamo.Abstractions;
using Imagegram.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Imagegram.Controllers
{
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] UserCreateRequest userCreateRequest, CancellationToken ct)
        {
            await _userRepository.Add(userCreateRequest.Login, ct);
            return Ok();
        }
    }
}