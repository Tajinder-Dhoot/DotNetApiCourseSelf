using Microsoft.AspNetCore.Mvc;
namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{

    DataContextDapper _dapper;

    public UsersController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")]
    public string TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()").ToString("yyyy-MM-dd");
    }

    [HttpGet("GetUsers/{testValue}")]
    public string[] GetUsers(string testValue)
    {
        return new string[] {"user1", "user2", testValue};
    }
}