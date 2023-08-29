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
    [Route("api/Registration")]
    [ApiController]
    public class RegistrationController : Controller
    {
        private readonly IRegistrationRepository _registrationRepo;
        private readonly IMapper _mapper;

        protected APIResponse _response;
        private string _userId;
        public RegistrationController(IRegistrationRepository registrationRepo, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _registrationRepo = registrationRepo;
            _mapper = mapper;
            _response = new();
            _userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }


        [HttpGet]
        [Route("GetAllRegistration")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllRegistration()
        {
            try
            {
                IEnumerable<Registration> registrationList = await _registrationRepo.GetAllAsync(u => (!u.IsDeleted), includeProperties : "CategoryMaster,StateMaster,CountryMaster");
                _response.Result = _mapper.Map<List<RegistrationDTO>>(registrationList);
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetRegistration(int registrationId)
        {
            try
            {
                if (registrationId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var registrationDetail = await _registrationRepo.GetAsync( u => u.RegistrationId == registrationId && u.IsDeleted == false);

                if (registrationDetail == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<RegistrationDTO>(registrationDetail);
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


        [HttpPost]
        [Route("CreateRegistration")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateRegistration([FromBody] RegistrationDTO registrationDTO)
        {
            try
            {
                if (await _registrationRepo.GetAsync(u => u.RegistrationId == registrationDTO.RegistrationId  && u.Email == registrationDTO.Email || u.RegistrationId == registrationDTO.RegistrationId || u.Email == registrationDTO.Email) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Registering Person already exists!");
                    return BadRequest(ModelState);
                }

                if (registrationDTO == null)
                {
                    return BadRequest(registrationDTO);
                }

                Registration registrationDetail = _mapper.Map<Registration>(registrationDTO);

                if(_userId == null)
                {
                    _userId = "0";
                }

                registrationDetail.CreatedOn = DateTime.Now;
                registrationDetail.CreatedBy = int.Parse(_userId);
                registrationDetail.UpdatedOn = DateTime.Now;
                registrationDetail.UpdatedBy = int.Parse(_userId);
                registrationDetail.IsDeleted = false;
                await _registrationRepo.CreateAsync(registrationDetail);

                _response.Result = _mapper.Map<RegistrationDTO>(registrationDetail);
                _response.StatusCode = HttpStatusCode.Created;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ResponseMessage = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete]
        [Route("RemoveRegistration")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> RemoveRegistration(int registrationId)
        {
            try
            {
                if (registrationId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var registrationDetail = await _registrationRepo.GetAsync(u => u.RegistrationId == registrationId);

                if (registrationDetail == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                registrationDetail.IsDeleted = true;
                await _registrationRepo.UpdateAsync(registrationDetail);

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
        [Route("UpdateRegistration")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateRegistration([FromBody] RegistrationDTO registrationDTO)
        {
            try
            {
                if (registrationDTO == null)
                {
                    return BadRequest();
                }

                if (await _registrationRepo.GetAsync(u => u.RegistrationId == registrationDTO.RegistrationId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Registration Id is Invalid");
                    return BadRequest(ModelState);
                }

                //if (await _registrationRepo.GetAsync(u => u.Email == registrationDTO.Email) != null)
                //{
                //    ModelState.AddModelError("ErrorMessages", "Registering Person already exists!");
                //    return BadRequest(ModelState);
                //}

                Registration registrationDetail = _mapper.Map<Registration>(registrationDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                registrationDetail.UpdatedOn = DateTime.Now;
                registrationDetail.UpdatedBy = int.Parse(_userId);
                registrationDetail.IsDeleted = false;
                await _registrationRepo.UpdateAsync(registrationDetail);

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


