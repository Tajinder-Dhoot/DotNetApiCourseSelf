using Microsoft.AspNetCore.Mvc;
using DotNetAPI.Models;
using DotNetAPI.Data;
using DotNetAPI.Dtos;
namespace DotNetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    DataContextDapper _dapper;

    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        string sql = @"
            SELECT  [UserId],
                    [FirstName],
                    [LastName],
                    [Email],
                    [Gender],
                    [Active]
            FROM  TutorialAppSchema.Users";
        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        string sql = @"
            SELECT  [UserId],
                    [FirstName],
                    [LastName],
                    [Email],
                    [Gender],
                    [Active]
            FROM  TutorialAppSchema.Users
            WHERE UserID = " + userId.ToString();

        User singleUser = _dapper.LoadDataSingle<User>(sql);
        return singleUser;
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
            UPDATE TutorialAppSchema.Users
            SET [FirstName] = '" + user.FirstName +
                "', [LastName] = '" + user.LastName +
                "', [Email] = '" + user.Email +
                "', [Gender] = '" + user.Gender +
                "', [Active] = '" + user.Active +
            "' WHERE [UserId] = " + user.UserId;

        Console.WriteLine(sql);

        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to update user");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.Users(
                [FirstName],
                [LastName],
                [Gender],
                [Email],
                [Active]
            ) VALUES (" +
                "'" + user.FirstName +
                "', '" + user.LastName +
                "', '" + user.Gender +
                "', '" + user.Email +
                "', '" + user.Active +
            "')";

        Console.WriteLine("Add User sql Query:" +sql);

        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to add new user");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"
            DELETE FROM TutorialAppSchema.Users
                WHERE UserId = " + userId.ToString();

        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User");
    }

    /***
     User Salary End Points
     ***/

    [HttpGet("UserSalary/{userId}")]
    public IEnumerable<UserSalary> GetUserSalary(int userId)
    {
        return _dapper.LoadData<UserSalary>(@"
            SELECT UserSalary.UserId
                    , UserSalary.Salary
            FROM  TutorialAppSchema.UserSalary
                WHERE UserId = " + userId.ToString());
    }

    [HttpPost("UserSalary")]
    public IActionResult PostUserSalary(UserSalary userSalaryForInsert)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserSalary (
                UserId,
                Salary
            ) VALUES (" + userSalaryForInsert.UserId.ToString()
                + ", " + userSalaryForInsert.Salary
                + ")";

        if (_dapper.ExecuteSqlWithRowCount(sql) > 0)
        {
            return Ok(userSalaryForInsert);
        }
        throw new Exception("Adding User Salary failed on save");
    }

    [HttpPut("UserSalary")]
    public IActionResult PutUserSalary(UserSalary userSalaryForUpdate)
    {
        string sql = "UPDATE TutorialAppSchema.UserSalary SET Salary=" 
            + userSalaryForUpdate.Salary
            + " WHERE UserId=" + userSalaryForUpdate.UserId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userSalaryForUpdate);
        }
        throw new Exception("Updating User Salary failed on save");
    }

    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        string sql = "DELETE FROM TutorialAppSchema.UserSalary WHERE UserId=" + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Deleting User Salary failed on save");
    }
}