using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Repository;
using ShoppingCartAPI.Repository.IRepository;
using System.Net;
using System.Security.Claims;

namespace ShoppingCartAPI.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        private string _userId;
        private readonly ApplicationDbContext _db;
        public UserController(IUserRepository userRepo, IHttpContextAccessor httpContextAccessor, IMapper mapper, ApplicationDbContext db)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _response = new();
            _userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            _db = db;
        }

        [HttpGet]
        [Route("GetRegistrationName")]
        public async Task<ActionResult<APIResponse>> GetRegistrationName(string prefix)
        {
            var searchTerm = prefix.ToUpper();
            var results = _db.Registration.ToList()
                .Where(u => u.FirstName.ToUpper().StartsWith(searchTerm));

            _response.Result = results;

            return Ok(_response);
        }


        [HttpGet]
        [Route("GetAllUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllUser()
        {
            try
            {
                IEnumerable<User> userList = await _userRepo.GetAllAsync( includeProperties: "RoleMaster,Registration");
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
        [Route("GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetUser(int userId)
        {
            try
            {
                if (userId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var userDetail = await _userRepo.GetUserAsync(userId);

                if (userDetail == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<UserDTO>(userDetail);
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
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("Register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            bool ifUserNameUnique = _userRepo.IsUniqueUser(userDTO);

            if (!ifUserNameUnique)
            {
                _response.ResponseMessage = new List<string>() { "Already Exists" };
                return BadRequest(_response);
            }

            if (_userId == null)
            {
                _userId = "0";
            }

            userDTO.CreatedOn = DateTime.Now;
            userDTO.CreatedBy = int.Parse(_userId);
            userDTO.UpdatedOn = DateTime.Now;
            userDTO.UpdatedBy = int.Parse(_userId);

            var user = await _userRepo.RegisterAsync(userDTO, _userId);

            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ResponseMessage.Add("Error while registering");
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("RemoveUser")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> RemoveUser(int userId)
        {
            try
            {
                if (userId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var userDetail = await _userRepo.GetUserAsync(userId);

                if (userDetail == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                if (_userId == null)
                {
                    _userId = "0";
                }

                userDetail.IsDeleted = true;
                await _userRepo.UpdateUserAsync(userDetail, _userId);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ResponseMessage = new List<string>() { ex.ToString() };
            }
            return _response;
        }


        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("UpdateUser")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateUser([FromBody] UserDTO userDTO)
        {
            try
            {
                if (userDTO == null)
                {
                    return BadRequest();
                }

                if (await _userRepo.GetUserAsync(userDTO.UserId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "User ID is Invalid");
                    return BadRequest(ModelState);
                }

                if (await _userRepo.GetAsync(u => u.RegistrationId == userDTO.RegistrationId && u.UserId != userDTO.UserId ) != null)
                {
                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                User model = _mapper.Map<User>(userDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                model.UpdatedOn = DateTime.Now;
                model.UpdatedBy = int.Parse(_userId);

                await _userRepo.UpdateUserAsync(model, _userId);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ResponseMessage = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("EnableUser")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> EnableUser(int userId)
        {
            try
            {
                if (userId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var userDTO = await _userRepo.GetUserForEnableAsync(userId);

                if (userDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                User model = _mapper.Map<User>(userDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                model.IsDeleted = false;
                await _userRepo.UpdateUserAsync(model, _userId);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ResponseMessage = new List<string>() { ex.ToString() };
            }
            return _response;
        }

    }
}


