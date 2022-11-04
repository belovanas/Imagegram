using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Domain;
using Dynamo.Abstractions;

namespace Dynamo.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IAmazonDynamoDB _client;
        private const string tableName = "Posts";

        public CommentRepository(IAmazonDynamoDB client)
        {
            _client = client;
        }
        
        public async Task Add(Comment comment, CancellationToken ct)
        { 
            var request = new PutItemRequest()
            {
                TableName = tableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    {
                        "PostId", new AttributeValue()
                        {
                            S = comment.PostId
                        }
                    },
                    {
                        "EntityId", new AttributeValue()
                        {
                            S = $"comment#{comment.Id}"
                        }
                    },
                    {
                        "User", new AttributeValue()
                        {
                            S = comment.User
                        }
                    },
                    {
                        "Content", new AttributeValue()
                        {
                            S = comment.Content
                        }
                    },
                    {
                        "CreatedAt", new AttributeValue()
                        {
                            S = comment.CreatedAt.ToString()
                        }
                    }
                }
            };
            await _client.PutItemAsync(request, ct);
        }

        public async Task Delete(string id, string postId, CancellationToken ct)
        {
            await _client.DeleteItemAsync(new DeleteItemRequest()
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>()
                {
                    {
                        "PostId", new AttributeValue()
                        {
                            S = postId
                        }
                    },
                    {
                        "EntityId", new AttributeValue()
                        {
                            S = $"comment#{id}"
                        }
                    }
                }
            }, ct);
        }
        
        public async Task<Comment> Get(string id, string postId, CancellationToken ct)
        {
            var response = await _client.GetItemAsync(new GetItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>()
                {
                    { 
                        "PostId", new AttributeValue 
                        {
                            S = postId
                        }},
                    { 
                        "EntityId", new AttributeValue
                        {
                            S = $"comment#{id}"
                        }}
                }
            }, ct);
            
            if (!response.IsItemSet)
            {
                throw new Exception($"Comment {id} isn't found");
            }
            
            return new Comment()
            {
                Id = response.Item["EntityId"].S.Split('#')[1],
                PostId = response.Item["PostId"].S,
                User = response.Item["User"].S,
                Content = response.Item["Content"].S,
                CreatedAt = DateTime.ParseExact(response.Item["CreatedAt"].S, "dd.MM.yyyy HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture)
            };
        }
    }
}