
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotNetAPI.Data;
using DotNetAPI.Dtos;
using DotNetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotNetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration config)
        {
            _config = config;
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" 
                    + userForRegistration.Email + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if(existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128/ 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                    string sqlAddAuth = @"
                        INSERT INTO TutorialAppSchema.Auth  ([Email],
                        [PasswordHash],
                        [PasswordSalt]) VALUES ('" + userForRegistration.Email +
                        "', @PasswordHash, @PasswordSalt)";

                    List<SqlParameter> sqlparameters = [];

                    SqlParameter passwordSaltParameter = new("@PasswordSalt", SqlDbType.VarBinary)
                    {
                        Value = passwordSalt
                    };

                    SqlParameter passwordHashParameter = new("@PasswordHash", SqlDbType.VarBinary)
                    {
                        Value = passwordHash
                    };

                    sqlparameters.Add(passwordSaltParameter);
                    sqlparameters.Add(passwordHashParameter);

                    if(_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlparameters))
                    {
                        string sqlAddUser = @"
                            INSERT INTO TutorialAppSchema.Users(
                                [FirstName],
                                [LastName],
                                [Email],
                                [Gender],
                                [Active]
                            ) VALUES (" +
                                "'" + userForRegistration.FirstName + 
                                "', '" + userForRegistration.LastName +
                                "', '" + userForRegistration.Email + 
                                "', '" + userForRegistration.Gender + 
                                "', 1)";

                        if(_dapper.ExecuteSql(sqlAddUser))
                        {
                            return Ok();
                        }
                        throw new Exception("failed to Add user!");
                    }
                    throw new Exception("Failed to Register User!");
                }
                throw new Exception("User already exists with the email provided!");
            }
            throw new Exception("Passwords do not match!");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" +
                    userForLogin.Email + "'";

            IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

            if(existingUsers.Any())
            {
                // get password hash and salt from DB for user
                string sqlPasswordHashAndSalt = @"SELECT" +
                    " [PasswordHash]," +
                    " [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '" + 
                    userForLogin.Email + "'";

                UserForLoginConfirmationDto userForConfirmation = 
                    _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlPasswordHashAndSalt);

                //get hash of password entered
                byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

                //compare hashed password filled by user and passowrd hash, if equals -> return Ok(), else throe exception
                // if(passwordHash.Equals(userForLoginConfirmation.PasswordHash)) won't work
                for (int index = 0; index < passwordHash.Length; index++)
                {
                    if(passwordHash[index] != userForConfirmation.PasswordHash[index])
                    {
                        return StatusCode(401, "Incorrect Password");
                    }
                }

                string userIdSql = @"
                    SELECT UserId FROM  TutorialAppSchema.Users WHERE Email = '" +
                    userForLogin.Email + "'";
                
                int userId = _dapper.LoadDataSingle<int>(userIdSql);

                return Ok(new Dictionary<string, string> {
                    {"token", _authHelper.CreateToken(userId)}
                });
            }

            return StatusCode(400, "User Not Found!");
        }

        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string userIdSql = @"
                SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" +
                User.FindFirst("userId")?.Value + "'"; // User.FindFisrt("userId") get userId associated with the claim.

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return _authHelper.CreateToken(userId);
        }
    }
}
