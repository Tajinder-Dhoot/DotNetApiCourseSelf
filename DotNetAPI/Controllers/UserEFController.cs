using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DotNetAPI.Models;
using DotNetAPI.Data;
using DotNetAPI.Dtos;
namespace DotNetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{

    DataContextEF _entityFramework;
    IMapper _mapper;

    public UserEFController(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);

        _mapper = new Mapper(new MapperConfiguration(cfg =>{
            cfg.CreateMap<UserToAddDto, User>();
        }));

    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _entityFramework.Users.ToList<User>();
        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        User? user = _entityFramework.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefault<User>();
        if (user != null)
        {
            return user;
        }

        throw new Exception("failed to find User");
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDb = _entityFramework.Users
            .Where(u => u.UserId == user.UserId)
            .FirstOrDefault<User>();

        if(userDb != null)
        {
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;
            userDb.Active = user.Active;
            if(_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Update User");
        }

        throw new Exception("Failed to Get User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        User userDb = _mapper.Map<User>(user);

        _entityFramework.Add(userDb);
        if(_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        }

        throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDb = _entityFramework.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefault<User>();
 
        if(userDb != null)
        {
            _entityFramework.Users.Remove(userDb);
            if(_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }

        throw new Exception("Failed to Get User");
    }
}