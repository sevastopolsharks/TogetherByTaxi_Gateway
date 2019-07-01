using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarLab.BusManager.Abstraction;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace SevSharks.Gateway.WebApi.Controllers
{
    /// <summary>
    /// EmployeeController
    /// </summary>
    [Route("api/v1/user")]
    [Authorize]
    [ApiController]
    public class UserController : BaseController<UserController>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UserController(ILogger<UserController> logger,
            IBusManager busManager, IMapper mapper) : base(logger, busManager, mapper)
        {
        }
        /// <summary>
        /// Get bonus account
        /// </summary>
        [HttpGet("info")]
        public Task<IActionResult> Info()
        {
            throw new NotImplementedException();
            //if (string.IsNullOrEmpty(CurrentUserId))
            //{
            //    return BadRequest("Неверно задан идентификатор пользователя");
            //}

            //var request = new AuthUserGetInfoRequest
            //{
            //    CurrentUserId = CurrentUserId
            //};

            //return await RequestResponse<AuthUserGetInfoRequest, AuthUserGetInfoResponse, UserInfo>(request);
        }
    }
}
