using Azure;
using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDTO _response;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
            _response = new();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegestirationrequestDTO model)
        {

            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty( errorMessage )) 
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);

            }
            return Ok(_response);
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO model)
        {

            var loginResponse = await _authService.Login(model);

            if(loginResponse.User == null)
            {
                _response.IsSuccess=false;
                _response.Message = "Username or Password is Incorrect";
                return BadRequest(_response);
            }
                 _response.Result = loginResponse;
            
            return Ok(_response);

        }
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(RegestirationrequestDTO model)
        {
            var assignSuccessful = await _authService.AssignRole(model.Email,model.Role.ToUpper());
            if (!assignSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or Password is Incorrect";
                return BadRequest(_response);
            }

            return Ok(_response);

        }
    }
}
  

