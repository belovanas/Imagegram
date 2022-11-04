using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Dynamo.Abstractions;
using Imagegram.Dto;
using Imagegram.Requests;
using Microsoft.AspNetCore.Authorization;
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
        public Task Create([FromBody] UserCreateRequest userCreateRequest, CancellationToken ct)
        {
            return _userRepository.Add(userCreateRequest.Login, ct);
        }
    }
}