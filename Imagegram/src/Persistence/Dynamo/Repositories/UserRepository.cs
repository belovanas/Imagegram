using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Dynamo.Abstractions;

namespace Dynamo.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IAmazonDynamoDB _client;
        private const string tableName = "Users";

        public UserRepository(IAmazonDynamoDB client)
        {
            _client = client;
        }

        public async Task Add(string login, CancellationToken ct)
        {
            var request = new PutItemRequest()
            {
                TableName = tableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    {
                        "Login", new AttributeValue()
                        {
                            S = login
                        }
                    },
                    {
                        "CreatedAt", new AttributeValue()
                        {
                            S = DateTime.Now.ToString()
                        }
                    }
                }
            };
            await _client.PutItemAsync(request, ct);
        }

        public async Task<bool> DoesExist(string login, CancellationToken ct)
        {
            var response = await _client.GetItemAsync(new GetItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>()
                {
                    { "Login", new AttributeValue
                    {
                        S = login
                    }}
                }
            }, ct);
            return response.IsItemSet;
        }
    }
}