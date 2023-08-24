using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Repository;
using ShoppingCartAPI.Repository.IRepository;
using System.Net;
using System.Security.Claims;

namespace ShoppingCartAPI.Controllers
{
    [Route("api/UsersAuth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        private string _userId;
        public AuthController(IUserRepository userRepo, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _response = new();
            _userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }


        [HttpGet]
        [Route("GetUsers")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetUsers()
        {
            try
            {
                IEnumerable<User> userList = await _userRepo.GetAllUserAsync();
                _response.Result = _mapper.Map<List<User>>(userList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ResponseMessage = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpGet]
        [Route("GetRegistration")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetRegistration()
        {
            try
            {
                IEnumerable<Registration> registrationList = await _userRepo.GetAllRegistrationAsync();
                _response.Result = _mapper.Map<List<Registration>>(registrationList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ResponseMessage = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            var loginResponse = await _userRepo.Login(loginRequest);

            if (loginResponse.UserRegistration == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ResponseMessage.Add("Username or password is incorrect");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationDTO registrationRequestDTO)
        {
            bool ifUserNameUnique = _userRepo.IsUniqueUser(registrationRequestDTO.Email);

            if (!ifUserNameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ResponseMessage.Add("Email already exists");
                return BadRequest(_response);
            }

            if (_userId == null)
            {
                _userId = "0";
            }

            var registeredPersonResult = await _userRepo.Register(registrationRequestDTO, _userId);

            if (registeredPersonResult == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ResponseMessage.Add("Error while registering");
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = registeredPersonResult;
            return Ok(_response);
        }


        //[Authorize(Roles = "Admin")]
        [HttpPost("UserRegister")]
        public async Task<IActionResult> UserRegister([FromBody] UserDTO registrationRequestDTO)
        {
            bool ifUserNameUnique = _userRepo.IsUniqueUser(registrationRequestDTO.UserName);

            if (!ifUserNameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ResponseMessage.Add("Email already exists");
                return BadRequest(_response);
            }

            if (_userId == null)
            {
                _userId = "0";
            }

            var result = await _userRepo.UserRegister(registrationRequestDTO, _userId);

            if (result == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ResponseMessage.Add("Error while registering");
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = result;
            return Ok(_response);
        }
    }
}


