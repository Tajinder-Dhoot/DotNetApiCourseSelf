using Microsoft.AspNetCore.Mvc;
namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{

    public UsersController()
    {

    }

    [HttpGet("GetUsers/{testValue}")]
    public string[] GetUsers(string testValue)
    {
        return new string[] {"user1", "user2", testValue};
    }
}