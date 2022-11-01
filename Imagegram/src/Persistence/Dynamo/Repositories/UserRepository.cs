using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Domain;
using Dynamo.Abstractions;
using Dynamo.Models;

namespace Dynamo.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DynamoDBContext _context;

        public UserRepository(IAmazonDynamoDB client)
        {
            _context = new DynamoDBContext(client);
        }

        public async Task Add(User user, CancellationToken ct)
        {
            var userDbModel = new UserDbModel
            {
                Id = Guid.NewGuid().ToString(),
                Name = user.Name,
                CreatedAt = DateTime.UtcNow
            };

            await _context.SaveAsync(userDbModel, ct);
        }

        public async Task<User> GetByName(string name, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("User's name should be filled");
            }

            var scanCondition = new ScanCondition("Name", ScanOperator.Equal, name);
            
            var result = await _context.ScanAsync<UserDbModel>(new []{scanCondition})
                .GetRemainingAsync(ct);
            if (result is null || !result.Any())
            {
                throw new ArgumentException($"Couldn't find user by name {name}");
            }

            var userDbModel = result.First();
            return new User
            {
                Id = userDbModel.Id,
                Name = userDbModel.Name
            };
        }
    }
}