using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Domain;
using Dynamo.Abstractions;

namespace Dynamo.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IAmazonDynamoDB _client;
        private const string tableName = "Posts";

        public PostRepository(
            IAmazonDynamoDB client)
        {
            _client = client;
        }
        
        public async Task Add(PostInfo postInfo, CancellationToken ct)
        {
            var request = new PutItemRequest()
            {
                TableName = tableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    {
                        "Id", new AttributeValue()
                        {
                            S = postInfo.Id
                        }
                    },
                    {
                        "User", new AttributeValue()
                        {
                            S = postInfo.User
                        }
                    },
                    {
                        "Caption", new AttributeValue()
                        {
                            S = postInfo.Caption
                        }
                    },
                    {
                        "CreatedAt", new AttributeValue()
                        {
                            S = postInfo.CreatedAt.ToString()
                        }
                    },
                }
            };
            await _client.PutItemAsync(request, ct);
        }
        
        public async Task Delete(string id, CancellationToken ct)
        {
            await _client.DeleteItemAsync(new DeleteItemRequest()
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>()
                {
                    {
                        "Id", new AttributeValue()
                        {
                            S = id
                        }
                    }
                }
            }, ct);
        }

        public async Task<PostInfo> Get(string id, CancellationToken ct)
        {
            var response = await _client.GetItemAsync(new GetItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>()
                {
                    { "Id", new AttributeValue
                    {
                        S = id
                    }}
                }
            }, ct);
            
            if (!response.IsItemSet)
            {
                throw new Exception($"Post {id} isn't found");
            }
            
            return new PostInfo
            {
                Id = response.Item["Id"].S,
                User = response.Item["User"].S,
                Caption = response.Item["Caption"].S,
                CreatedAt = DateTime.ParseExact(response.Item["CreatedAt"].S, "dd.MM.yyyy HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture)
            };
        }
    }
}