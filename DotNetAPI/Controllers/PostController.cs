using DotnetAPI.Dtos;
using DotnetAPI.Models;
using DotNetAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _config = config;
            _dapper = new DataContextDapper(config);
        }

        [AllowAnonymous]
        [HttpGet("GetPosts")]
        public IEnumerable<Post> GetPosts()
        {
            string sqlGetAllPosts = "SELECT * FROM TutorialAppSchema.Posts";

            IEnumerable<Post> posts = _dapper.LoadData<Post>(sqlGetAllPosts);
            return posts;
        }

        [AllowAnonymous]
        [HttpGet("{postId}")]
        public Post Post(int postId)
        {
            string sql = "SELECT * FROM TutorialAppSchema.Posts WHERE PostId = " + postId.ToString();
            Post post = _dapper.LoadDataSingle<Post>(sql);
            return post;
        }

        [AllowAnonymous]
        [HttpGet("UserPosts/{userId}")]
        public IEnumerable<Post> GetPostByUser(int userId)
        {
            string sql = "SELECT * FROM TutorialAppSchema.Posts WHERE UserId = " + userId.ToString();
            IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);
            return posts;
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = "SELECT * FROM TutorialAppSchema.Posts WHERE UserId = " +
                User.FindFirst("userId")?.Value;

            IEnumerable<Post> myPosts = _dapper.LoadData<Post>(sql);
            return myPosts;
        }

        [HttpPost("Post")]
        public IActionResult AddPost(PostToAddDto postToAdd)
        {
            string sqlInsertPost = @"INSERT INTO TutorialAppSchema.Posts (
                UserId, 
                PostTitle, 
                PostContent, 
                PostCreated, 
                PostUpdated) VALUES (" +
                    this.User.FindFirst("userId")?.Value + 
                    ", '" + postToAdd.PostTitle +
                    "', '" + postToAdd.PostContent +
                    "', GetDate()" +
                    ", GetDate());";

            if(_dapper.ExecuteSql(sqlInsertPost))
            {
                return Ok();
            }

            throw new Exception("Failed to create new Post!");
        }

        [HttpPut("Post")]
        public IActionResult EditPost(PostToEditDto postToEdit)
        {
            string sqlUpdatePost = @"UPDATE TutorialAppSchema.Posts
                SET [PostTitle] = '" + postToEdit.PostTitle
                    + "', [PostContent] = '" + postToEdit.PostContent
                    + "', [PostUpdated] = GETDATE()"
                + " WHERE PostId = " + postToEdit.PostId.ToString()
                + " AND UserId = " + this.User.FindFirst("userId")?.Value;

            if(_dapper.ExecuteSql(sqlUpdatePost))
            {
                return Ok();
            }

            throw new Exception("Failed to update the Post!");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string? userIdFromToken = this.User.FindFirst("userId")?.Value;
            string sqlDeletePost = @"DELETE TutorialAppSchema.Posts" + 
                                        " WHERE PostId = " + postId.ToString() + 
                                        " AND UserId = " + userIdFromToken;

            if(_dapper.ExecuteSql(sqlDeletePost))
            {
                return Ok();
            }

            // if postId and userId are not in sync
            string sqlUserId = "SELECT UserId FROM TutorialAppSchema.Posts WHERE PostId = " + postId;
            if(_dapper.LoadDataSingle<string>(sqlUserId) != userIdFromToken && userIdFromToken != null)
            {
                return StatusCode(403, "Post does not belong to the user");
            }

            throw new Exception("Failed to delete the Post!");
        }
    }
}
