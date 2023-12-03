
using Microsoft.AspNetCore.Mvc;
using DotNetAPI.Data;

namespace DotNetAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly DataContextDapper _dapper;

        public AuthController(IConfiguration config)
        {
            _config = config;
            _dapper = new DataContextDapper(config);
        }
    }
}