using Microsoft.AspNetCore.Mvc;
using DotNetAPI.Models;
using DotNetAPI.Data;
using DotNetAPI.Dtos;
namespace DotNetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserJobInfoController : ControllerBase
{
    DataContextDapper _dapper;

    public UserJobInfoController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("GetUsersInfo")]
    public IEnumerable<UserJobInfo> GetUsersInfo()
    {
        string sql = "SELECT  * FROM  TutorialAppSchema.UserJobInfo";
        IEnumerable<UserJobInfo> usersJobInfo = _dapper.LoadData<UserJobInfo>(sql);
        return usersJobInfo;
    }

    [HttpGet("GetUserInfo/{userId}")]
    public UserJobInfo GetUserInfo(int userId)
    {
        string sql = @"
            SELECT  [UserId]" +
                ", [JobTitle]" +
                ", [Department]" +
                " FROM  TutorialAppSchema.UserJobInfo" +
                " WHERE UserId = " +userId.ToString();
        UserJobInfo userJobInfo = _dapper.LoadDataSingle<UserJobInfo>(sql);
        return userJobInfo;
    }

    [HttpPost("AddUserInfo")]
    public IActionResult AddUserInfo(UserJobInfo userJobInfo)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserJobInfo(" +
                "[UserId]," +
                " [JobTitle]," +
                " [Department]" +
            ") VALUES (" +
                userJobInfo.UserId +
                ", '" + userJobInfo.JobTitle +
                "', '" + userJobInfo.Department + "')";

        Console.WriteLine("AddUserInfo SQL Query: " + sql);

        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("failed to add New User Job Info");
    }

    [HttpPut("EditUserInfo")]
    public IActionResult EditUserInfo(UserJobInfo userJobInfo)
    {
        string sql = @"
            UPDATE TutorialAppSchema.UserJobInfo" +
                " SET [JobTitle] = '" + userJobInfo.JobTitle +
                "', [Department] = '" + userJobInfo.Department +
                "' WHERE [UserId] = " + userJobInfo.UserId.ToString();

        Console.WriteLine("Edit User Info SQL Query: " + sql);

        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("failed to edit New User Job Info");
    }

    [HttpDelete("DeleteUserInfo/{userId}")]
    public IActionResult DeleteUserInfo(int userId)
    {
        string sql = @"
            DELETE FROM TutorialAppSchema.UserJobInfo WHERE UserId = " + userId.ToString();

        Console.WriteLine("Delete User Info SQL Query: " + sql);

        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("failed to Delete User Job Info");
    }
}