using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Repository;
using ShoppingCartAPI.Repository.IRepository;
using System.Data;
using System.Net;
using System.Security.Claims;

namespace ShoppingCartAPI.Controllers
{
    [Route("api/MenuRoleMapping")]
    [ApiController]
    public class MenuRoleMappingController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IMenuRoleMappingRepository _dbMenuRoleMapping;
        private string _userId;

        public MenuRoleMappingController(IMenuRoleMappingRepository _MenuRoleMappingRepository, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbMenuRoleMapping = _MenuRoleMappingRepository;
            _mapper = mapper;
            _response = new();
            _userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        [Route("GetAllMenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllMenuRoleMapping()
        {
            try
            {
                IEnumerable<MenuRoleMapping> roleList = await _dbMenuRoleMapping.GetAllAsync();
                _response.Result = _mapper.Map<List<MenuRoleMappingDTO>>(roleList);
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
        [Route("GetMenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetMenuRoleMapping(int MenuRoleMappingId)
        {
            try
            {
                if (MenuRoleMappingId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var MenuRoleMapping = await _dbMenuRoleMapping.GetAsync(u => u.MenuRoleMappingId == MenuRoleMappingId && u.IsDeleted == false);

                if (MenuRoleMapping == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<MenuRoleMappingDTO>(MenuRoleMapping);
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
        [Route("CreateMenuRoleMapping")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateMenuRoleMapping([FromBody] MenuRoleMappingDTO MenuRoleMappingDTO)
        {
            try
            {
                //if (await _dbMenuRoleMapping.GetAsync(u => u.MenuRoleMappingName == MenuRoleMappingDTO.MenuRoleMappingName) != null)
                {
                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                if (MenuRoleMappingDTO == null)
                {
                    return BadRequest(MenuRoleMappingDTO);
                }

                MenuRoleMapping MenuRoleMapping = _mapper.Map<MenuRoleMapping>(MenuRoleMappingDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                MenuRoleMapping.CreatedOn = DateTime.Now;
                MenuRoleMapping.CreatedBy = int.Parse(_userId);
                MenuRoleMapping.UpdatedOn = DateTime.Now;
                MenuRoleMapping.UpdatedBy = int.Parse(_userId);
                MenuRoleMapping.IsDeleted = false;
                await _dbMenuRoleMapping.CreateAsync(MenuRoleMapping);

                _response.Result = _mapper.Map<MenuRoleMappingDTO>(MenuRoleMapping);
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
        [Authorize(Roles = "Admin")]
        [Route("RemoveMenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> RemoveMenuRoleMapping(int MenuRoleMappingId)
        {
            try
            {
                if (MenuRoleMappingId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var MenuRoleMapping = await _dbMenuRoleMapping.GetAsync(u => u.MenuRoleMappingId == MenuRoleMappingId && u.IsDeleted == false);

                if (MenuRoleMapping == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                MenuRoleMapping.IsDeleted = true;
                await _dbMenuRoleMapping.UpdateAsync(MenuRoleMapping);

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
        [Route("UpdateMenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateMenuRoleMapping([FromBody] MenuRoleMappingDTO MenuRoleMappingDTO)
        {
            try
            {
                if (MenuRoleMappingDTO == null)
                {
                    return BadRequest();
                }

                int MenuRoleMappingId = MenuRoleMappingDTO.MenuRoleMappingId;

                if (await _dbMenuRoleMapping.GetAsync(u => u.MenuRoleMappingId == MenuRoleMappingId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "MenuRoleMapping ID is Invalid!");
                    return BadRequest(ModelState);
                }

                //if (await _dbMenuRoleMapping.GetAsync(u => u.MenuRoleMappingName == MenuRoleMappingDTO.MenuRoleMappingName && u.MenuRoleMappingId != MenuRoleMappingDTO.MenuRoleMappingId) != null)
                {
                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                MenuRoleMapping model = _mapper.Map<MenuRoleMapping>(MenuRoleMappingDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                model.UpdatedOn = DateTime.Now;
                model.UpdatedBy = int.Parse(_userId);
                model.IsDeleted = false;
                await _dbMenuRoleMapping.UpdateAsync(model);

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
        [Route("EnableMenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> EnableMenuRoleMapping(int MenuRoleMappingId)
        {
            try
            {
                if (MenuRoleMappingId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var MenuRoleMappingDTO = await _dbMenuRoleMapping.GetAsync(u => u.MenuRoleMappingId == MenuRoleMappingId && u.IsDeleted == true);

                if (MenuRoleMappingDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                MenuRoleMapping model = _mapper.Map<MenuRoleMapping>(MenuRoleMappingDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                model.IsDeleted = false;
                await _dbMenuRoleMapping.UpdateAsync(model);

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
