using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain;
using Dynamo.Abstractions;
using S3;

namespace Application
{
    public class PostService : IPostService
    {
        private readonly IImageUploader _imageUploader;
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository, IImageUploader imageUploader)
        {
            _postRepository = postRepository;
            _imageUploader = imageUploader;
        }

        public async Task Create(Post post, Stream image, CancellationToken ct)
        {
            await _imageUploader.UploadFile(image, post.Id, ct);
            await _postRepository.Add(post, ct);
        }

        public async Task Delete(string id, CancellationToken ct)
        {
            await _postRepository.Delete(id, ct);
            await _imageUploader.DeleteFile(id, ct);
        }

        public async Task<Post> Get(string id, CancellationToken ct)
        {
            var imageLink = _imageUploader.GetLinkToFile(id);
            var post = await _postRepository.Get(id, ct);
            post.ImageLink = imageLink;
            return post;
        }

        public async Task<List<Post>> GetPage(int limit, int offset, CancellationToken ct)
        {
            var posts = await _postRepository.GetAll(ct);
            var sortedPosts = posts.OrderByDescending(x => x.Comments.Count);
            var pagedPosts = sortedPosts.Skip(offset).Take(limit).ToList();
            return pagedPosts;
        }
    }
}