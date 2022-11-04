using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private const string tableName = "Posts";
        private readonly IAmazonDynamoDB _client;

        public PostRepository(
            IAmazonDynamoDB client)
        {
            _client = client;
        }

        public async Task Add(Post post, CancellationToken ct)
        {
            var request = new PutItemRequest
            {
                TableName = tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    {
                        "PostId", new AttributeValue
                        {
                            S = post.Id
                        }
                    },
                    {
                        "EntityId", new AttributeValue
                        {
                            S = $"post#{post.Id}"
                        }
                    },
                    {
                        "User", new AttributeValue
                        {
                            S = post.User
                        }
                    },
                    {
                        "Caption", new AttributeValue
                        {
                            S = post.Caption
                        }
                    },
                    {
                        "CreatedAt", new AttributeValue
                        {
                            S = post.CreatedAt.ToString()
                        }
                    }
                }
            };
            await _client.PutItemAsync(request, ct);
        }

        public async Task Delete(string id, CancellationToken ct)
        {
            await _client.DeleteItemAsync(new DeleteItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    {
                        "PostId", new AttributeValue
                        {
                            S = id
                        }
                    },
                    {
                        "EntityId", new AttributeValue
                        {
                            S = $"post#{id}"
                        }
                    }
                }
            }, ct);
        }

        public async Task<Post> Get(string id, CancellationToken ct)
        {
            var response = await _client.QueryAsync(new QueryRequest
            {
                TableName = tableName,
                KeyConditionExpression = "PostId = :postId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {
                        ":postId", new AttributeValue
                        {
                            S = id
                        }
                    }
                }
            }, ct);

            if (!response.Items.Any()) throw new Exception($"Post with id {id} isn't found");

            var post = MapPosts(response.Items);
            return post.Single();
        }

        public async Task<List<Post>> GetAll(CancellationToken ct)
        {
            var response = await _client.ScanAsync(new ScanRequest
            {
                TableName = tableName
            }, ct);

            return MapPosts(response.Items);
        }

        private List<Post> MapPosts(List<Dictionary<string, AttributeValue>> items)
        {
            var commentRaws = items.Where(x => x["EntityId"].S.StartsWith("comment"));
            var comments = commentRaws.Select(comment => new Comment
            {
                Id = comment["EntityId"].S.Split('#')[1],
                PostId = comment["PostId"].S,
                Content = comment["Content"].S,
                User = comment["User"].S,
                CreatedAt = DateTime.ParseExact(comment["CreatedAt"].S, "dd.MM.yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture)
            }).ToList();

            var postRaws = items.Where(x => x["EntityId"].S.StartsWith("post"));
            var postModels = postRaws.Select(post => new Post
            {
                Id = post["PostId"].S,
                Caption = post["Caption"].S,
                User = post["User"].S,
                CreatedAt = DateTime.ParseExact(post["CreatedAt"].S, "dd.MM.yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture),
                Comments = comments
            }).ToList();
            var postModelsMap = postModels.ToDictionary(x => x.Id, x => x);

            foreach (var commentByPost in comments.GroupBy(x => x.PostId))
                postModelsMap[commentByPost.Key].Comments = commentByPost.ToList();

            return postModels;
        }
    }
}