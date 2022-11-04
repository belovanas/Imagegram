using System;
using System.IO;
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
        private readonly ICommentRepository _commentRepository;

        public PostService(
            IImageUploader imageUploader, 
            IPostRepository postRepository,
            ICommentRepository commentRepository)
        {
            _imageUploader = imageUploader;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
        }

        public async Task Save(Post post, Stream image, CancellationToken ct)
        {
            await _postRepository.Add(post.Info, ct);
            try
            {
                await _imageUploader.UploadFile(image, post.Info.Id, ct);
            }
            catch (Exception)
            {
                await _postRepository.Delete(post.Info.Id, ct);
                throw;
            }
        }

        public async Task<Post> GetById(string id, CancellationToken ct)
        {
            var postInfo = await _postRepository.Get(id, ct);
            var comments = await _commentRepository.GetByPostId(id, ct);
            return new Post
            {
                Info = postInfo,
                Image = image,
                Comments = comments
            };
        }
        
        public Task<Stream> GetImageForPost(string id, CancellationToken ct)
        {
            return _imageUploader.GetFile(id, ct);
        }
    }
}
