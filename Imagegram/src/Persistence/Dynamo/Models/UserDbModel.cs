using System;
using Amazon.DynamoDBv2.DataModel;

namespace Dynamo.Models
{
    [DynamoDBTable("Users")]
    public class UserDbModel
    {
        [DynamoDBProperty("Id")]
        public string Id { get; set; }

        [DynamoDBProperty("Name")]
        public string Name { get; set; }
        
        [DynamoDBProperty("CreatedAt")]
        public DateTime CreatedAt { get; set; }
    }
}