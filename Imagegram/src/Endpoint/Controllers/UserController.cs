using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Dynamo.Abstractions;
using Imagegram.Dto;
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
        public Task Create([FromBody] UserCreateModel userCreateModel, CancellationToken ct)
        {
            var user = new User()
            {
                Name = userCreateModel.Name
            };
            return _userRepository.Add(user, ct);
        }
    }
}